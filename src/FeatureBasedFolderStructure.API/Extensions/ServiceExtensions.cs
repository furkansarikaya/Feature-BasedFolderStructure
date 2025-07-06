using System.Reflection;
using System.Security.Claims;
using System.Text;
using FeatureBasedFolderStructure.Application.Common.Behaviors;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Settings;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Mappings;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Validators;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Infrastructure.Features.Auth.Repositories;
using FeatureBasedFolderStructure.Infrastructure.Persistence;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using FluentValidation;
using FS.AspNetCore.ResponseWrapper;
using FS.AspNetCore.ResponseWrapper.Middlewares;
using FS.AspNetCore.ResponseWrapper.Models;
using FS.AutoServiceDiscovery.Extensions.DependencyInjection;
using FS.EntityFramework.Library.FluentConfiguration;
using FS.Mediator.Extensions;
using FS.Mediator.Features.Backpressure.Models.Enums;
using FS.Mediator.Features.CircuitBreaker.Models.Enums;
using FS.Mediator.Features.HealthChecking.Models.Enums;
using FS.Mediator.Features.Retry.Models.Enums;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Versioning;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;

namespace FeatureBasedFolderStructure.API.Extensions;

public static class ServiceExtensions
{
    private static void AddOpenApiDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi(options =>
        {
            options.AddDocumentTransformer((document, context, cancellationToken) =>
            {
                document.Info = new OpenApiInfo
                {
                    Title = "Feature-Based Folder Structure API Documentation | v1",
                    Version = "v1",
                    Description = "API for Feature-Based Folder Structure.",
                    Contact = new OpenApiContact
                    {
                        Name = "Furkan SARIKAYA",
                        Email = "furkannsarikaya@gmail.com",
                        Url = new Uri("https://github.com/furkansarikaya/Feature-BasedFolderStructure")
                    }
                };
                return Task.CompletedTask;
            });
            options.AddDocumentTransformer<BearerSecuritySchemeTransformer>();
            options.AddOperationTransformer<AddHeaderParameterOpenApiOperationTransformer>();
        });
    }

    private static void AddFSMediatorServices(this IServiceCollection services)
    {
        services.AddFSMediator(Assembly.GetAssembly(typeof(CreateProductCommand))!)
            .AddRetryBehavior(RetryPreset.Database)
            .AddCircuitBreakerBehavior(CircuitBreakerPreset.ExternalApi)
            .AddLoggingBehavior()
            .AddPerformanceBehavior()
            .AddStreamingResiliencePackage()                  // Complete streaming protection
            .AddStreamingBackpressureBehavior(BackpressurePreset.Analytics)  // Handle load spikes
            .AddStreamingHealthCheckBehavior(HealthCheckPreset.LongRunning)      // Monitor health

            .AddPipelineBehavior(typeof(AuthorizationBehavior<,>))
            .AddPipelineBehavior(typeof(ValidationBehavior<,>));
    }

    private static void AddCoreServices(this IServiceCollection services, string environmentName)
    {
        services.AddCors(options =>
        {
            options.AddPolicy("AllowAll",
                builder => builder
                    .AllowAnyMethod()
                    .AllowCredentials()
                    .SetIsOriginAllowed((host) => true)
                    .AllowAnyHeader());
        });
        services.AddHttpContextAccessor();
        services.AddControllers(options =>
        {
           options.Filters.Add((new ProducesResponseTypeAttribute(typeof(ApiResponse<object>), StatusCodes.Status400BadRequest)));
           options.Filters.Add((new ProducesResponseTypeAttribute(typeof(ApiResponse<object>), StatusCodes.Status401Unauthorized)));
           options.Filters.Add((new ProducesResponseTypeAttribute(typeof(ApiResponse<object>), StatusCodes.Status403Forbidden)));
           options.Filters.Add((new ProducesResponseTypeAttribute(typeof(ApiResponse<object>), StatusCodes.Status404NotFound)));
           options.Filters.Add((new ProducesResponseTypeAttribute(typeof(ApiResponse<object>), StatusCodes.Status500InternalServerError)));
        });

        services.AddResponseWrapper(options =>
        {
            options.DateTimeProvider = () => DateTime.Now;
            options.EnableQueryStatistics= environmentName != "Production";
        });
        
        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader
                .Combine(
                    new UrlSegmentApiVersionReader()
                );
        });
        services.AddVersionedApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });
        
        services.AddEndpointsApiExplorer();
        
        services.AddTransient<GlobalExceptionHandlingMiddleware>();
    }

    private static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration, string environmentName)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration
                    .GetConnectionString("DefaultConnection"))
                .UseSnakeCaseNamingConvention());

       services.AddFSEntityFramework<ApplicationDbContext>()
            .WithAudit()
            .UsingUserProvider(provider => provider.GetRequiredService<ICurrentUserService>().UserId,
                provider => provider.GetRequiredService<IDateTime>().Now)
            .WithDetailedLogging(environmentName != "Production")
            .WithSoftDelete()
            .Build();

        services.AddScoped<ApplicationDbContextInitialiser>();
    }

    private static void AddCustomServices(this IServiceCollection services)
    {
        services.Configure<JwtSettings>(services.BuildServiceProvider().GetService<IConfiguration>()!
            .GetSection(nameof(JwtSettings)));
    }

    private static void AddAssemblyServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateProductCommandValidator)));
        services.AddAutoMapper(Assembly.GetAssembly(typeof(ProductMappingProfile)));
    }
    
    private static void AddAuthorizationPolicies(this IServiceCollection services,IConfiguration configuration)
    {
        var jwtSettings = configuration.GetRequiredSection(nameof(JwtSettings)).Get<JwtSettings>()!;

        services
            .AddAuthorization()
            .AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer = jwtSettings.Issuer,
                    ValidAudience = jwtSettings.Audience,
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(jwtSettings.Key))
                };

                options.Events = new JwtBearerEvents
                {
                    OnTokenValidated = async context =>
                    {
                        var tokenService = context.HttpContext.RequestServices
                            .GetRequiredService<ITokenService>();

                        var nameIdentifier = context.Principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                        _ = Guid.TryParse(nameIdentifier, out var userId);
                        var token = context.Request.Headers["Authorization"]
                            .FirstOrDefault()?.Split(" ").Last() ?? "";

                        if (!await tokenService.ValidateTokenAsync(userId, token, TokenType.AccessToken))
                            context.Fail("Token is invalid");
                    }
                };
            });
    }

    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration, string environmentName)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddCoreServices(environmentName);
        services.AddOpenApiDocumentation();
        services.AddFSMediatorServices();
        services.AddDatabaseContext(configuration, environmentName);
        
        // Auto service registration - tüm layer'ları tara

        services.ConfigureAutoServices()
            .FromAssemblies(Assembly.GetAssembly(typeof(Domain.ValueObjects.Catalogs.Money))!,
                Assembly.GetAssembly(typeof(Application.Common.Attributes.RequiresClaimAttribute))!,
                Assembly.GetAssembly(typeof(ApplicationUserRepository))!)
            .WithProfile(environmentName)
            .WithConfiguration(configuration)
            .WithLogging(environmentName != "Production")
            .WithPerformanceOptimizations()
            .ForTestEnvironment(false)
            .Apply();
        
        services.AddCustomServices();
        services.AddAssemblyServices();
        services.AddAuthorizationPolicies(configuration);
    }
}