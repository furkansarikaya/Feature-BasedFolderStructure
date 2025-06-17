using System.Reflection;
using System.Security.Claims;
using System.Text;
using FeatureBasedFolderStructure.API.Common;
using FeatureBasedFolderStructure.API.Configuration;
using FeatureBasedFolderStructure.Application.Common.Behaviors;
using FeatureBasedFolderStructure.Application.Common.Extensions;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models.Responses;
using FeatureBasedFolderStructure.Application.Common.Settings;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Mappings;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Validators;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Infrastructure.Persistence;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Interceptors;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories.Users;
using FeatureBasedFolderStructure.Infrastructure.Services;
using FluentValidation;
using MediatR;
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

    private static void AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(CreateProductCommand))!);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(AuthorizationBehavior<,>));
        });
    }

    private static void AddCoreServices(this IServiceCollection services)
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
        services.ConfigureApiFilters(); // Auto wrapper filter'ı ekler
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

        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
        services.AddTransient<GlobalExceptionHandlingMiddleware>();
    }

    private static void AddDatabaseContext(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<ApplicationDbContext>(options =>
            options.UseNpgsql(configuration
                .GetConnectionString("DefaultConnection"))
                .EnableSensitiveDataLogging(
                    Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
                .UseSnakeCaseNamingConvention());
        
        services.AddScoped<ApplicationDbContextInitialiser>();
    }

    private static void AddCustomServices(this IServiceCollection services)
    {
        services.Configure<JwtSettings>(services.BuildServiceProvider().GetService<IConfiguration>()
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

        services.AddCoreServices();
        services.AddOpenApiDocumentation();
        services.AddMediatRServices();
        services.AddDatabaseContext(configuration);
        
        // Auto service registration - tüm layer'ları tara
        services.AddAutoServices(options =>
            {
                // Environment'a göre profile belirle
                options.Profile = environmentName;
                options.Configuration = configuration;
                options.EnableLogging = environmentName != "Production";
                options.IsTestEnvironment = false;
            }, 
            // Hangi assembly'lerin taranacağını belirt
            Assembly.GetAssembly(typeof(FeatureBasedFolderStructure.Domain.Common.Attributes.ServiceRegistrationAttribute))!, // Domain
            Assembly.GetAssembly(typeof(FeatureBasedFolderStructure.Application.Common.Extensions.ServiceCollectionExtensions))!, // Application  
            Assembly.GetAssembly(typeof(ApplicationUserRepository))!, // Infrastructure
            Assembly.GetExecutingAssembly() // API
        );
        
        services.AddCustomServices();
        services.AddAssemblyServices();
        services.AddAuthorizationPolicies(configuration);
    }
}