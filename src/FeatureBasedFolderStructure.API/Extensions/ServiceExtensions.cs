using System.Reflection;
using FeatureBasedFolderStructure.Application.Common.Behaviors;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.Products.Mappings;
using FeatureBasedFolderStructure.Application.Features.Products.Validators;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Interceptors;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Repositories;
using FeatureBasedFolderStructure.Infrastructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.OpenApi.Models;

namespace FeatureBasedFolderStructure.API.Extensions;

public static class ServiceExtensions
{
    private static void AddSwaggerDocumentation(this IServiceCollection services)
    {
        services.AddOpenApi();
        services.AddSwaggerGen(c => 
        { 
            c.SwaggerDoc("v1", new OpenApiInfo { Title = "FeatureBasedFolderStructure.API", Version = "v1" }); 
        });
    }

    private static void AddMediatRServices(this IServiceCollection services)
    {
        services.AddMediatR(cfg =>
        {
            cfg.RegisterServicesFromAssembly(Assembly.GetAssembly(typeof(CreateProductCommand))!);
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));
            cfg.AddBehavior(typeof(IPipelineBehavior<,>), typeof(LoggingBehavior<,>));
        });
    }

    private static void AddCoreServices(this IServiceCollection services)
    {
        services.AddHttpContextAccessor();
        services.AddControllers();
        services.AddEndpointsApiExplorer();

        services.AddScoped<IDateTime, DateTimeService>();
        services.AddScoped<ICurrentUserService, CurrentUserService>();
        services.AddScoped<AuditableEntitySaveChangesInterceptor>();
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
    }

    private static void AddCustomServices(this IServiceCollection services)
    {
        // Custom service registrations can be added here
    }

    private static void AddAssemblyServices(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(Assembly.GetAssembly(typeof(CreateProductCommandValidator)));
        services.AddAutoMapper(Assembly.GetAssembly(typeof(ProductMappingProfile)));
    }

    public static void AddApiServices(this IServiceCollection services, IConfiguration configuration)
    {
        AppContext.SetSwitch("Npgsql.EnableLegacyTimestampBehavior", true);

        services.AddCoreServices();
        services.AddSwaggerDocumentation();
        services.AddMediatRServices();
        services.AddDatabaseContext(configuration);
        services.AddRepositories();
        services.AddCustomServices();
        services.AddAssemblyServices();
    }
}