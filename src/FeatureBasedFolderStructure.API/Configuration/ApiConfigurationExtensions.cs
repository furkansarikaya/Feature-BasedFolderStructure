using FeatureBasedFolderStructure.API.Filters;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Configuration;

/// <summary>
/// API configuration extension'ları.
/// Filter registration ve API behavior configuration burada yapılır.
/// </summary>
public static class ApiConfigurationExtensions
{
    /// <summary>
    /// API filter'larını configure eder.
    /// Bu method Program.cs'den çağrılır.
    /// </summary>
    public static IServiceCollection ConfigureApiFilters(this IServiceCollection services)
    {
        services.Configure<MvcOptions>(options =>
        {
            // Auto API Response Wrapper filter'ını tüm controller'lara uygula
            options.Filters.Add<ApiResponseWrapperFilter>();
            
            // Diğer global filter'lar da burada eklenebilir
            // options.Filters.Add<ValidationFilter>();
            // options.Filters.Add<AuditLogFilter>();
        });
        
        return services;
    }
}
