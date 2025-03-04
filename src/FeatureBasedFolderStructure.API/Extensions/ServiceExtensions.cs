using System.Reflection;
using System.Security.Claims;
using System.Text;
using FeatureBasedFolderStructure.API.Common;
using FeatureBasedFolderStructure.Application.Common.Behaviors;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.Products.Mappings;
using FeatureBasedFolderStructure.Application.Features.Products.Rules;
using FeatureBasedFolderStructure.Application.Features.Products.Validators;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Interceptors;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;
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
                    Description = "API for Feature-Based Folder Structure."
                };
                return Task.CompletedTask;
            });
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
        services.AddHttpContextAccessor();
        services.AddControllers();
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
                .UseSnakeCaseNamingConvention());
    }

    private static void AddRepositories(this IServiceCollection services)
    {
        services.AddScoped<IProductRepository, ProductRepository>();
        services.AddScoped<ICategoryRepository, CategoryRepository>();
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IUserTokenRepository, UserTokenRepository>();
        services.AddScoped<IRoleRepository, RoleRepository>();
    }

    private static void AddCustomServices(this IServiceCollection services)
    {
        services.AddScoped<ITokenService, TokenService>();
        services.Configure<JwtSettings>(services.BuildServiceProvider().GetService<IConfiguration>()
            .GetSection(nameof(JwtSettings)));
    }
    
    private static void AddBusinessRules(this IServiceCollection services)
    {
        services.AddScoped<ProductBusinessRules>();
    }

    private static void AddAssemblyServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateProductCommandValidator)));
        services.AddAutoMapper(Assembly.GetAssembly(typeof(ProductMappingProfile)));
    }
    
    private static void AddAuthorizationPolicies(this IServiceCollection services,IConfiguration configuration)
    {
        var jwtSettings = configuration.GetRequiredSection(nameof(JwtSettings)).Get<JwtSettings>()!;
        
        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = true,
                    ValidateAudience = true,
                    ValidateLifetime = true,
                    ValidateIssuerSigningKey = true,
                    ValidIssuer =jwtSettings.Issuer,
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
        services.AddAuthorization();
    }

    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddCoreServices();
        services.AddOpenApiDocumentation();
        services.AddMediatRServices();
        services.AddDatabaseContext(configuration);
        services.AddRepositories();
        services.AddBusinessRules();
        services.AddCustomServices();
        services.AddAssemblyServices();
        services.AddAuthorizationPolicies(configuration);
    }
}