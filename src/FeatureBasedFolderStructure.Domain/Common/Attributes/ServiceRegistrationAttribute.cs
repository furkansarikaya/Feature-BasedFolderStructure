using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Domain.Common.Attributes;

/// <summary>
/// Service'lerin otomatik DI registration'ı için kullanılan attribute.
/// Domain layer'da çünkü bu bir cross-cutting concern ve tüm layer'lar tarafından kullanılıyor.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class ServiceRegistrationAttribute(ServiceLifetime lifetime) : Attribute
{
    public ServiceLifetime Lifetime { get; } = lifetime;
    public int Order { get; set; } = 0;
    public Type? ServiceType { get; set; } // Eğer farklı bir interface implement etmek istiyorsak
    public string? Profile { get; set; } // Development, Production gibi profile'lar için
    public bool IgnoreInTests { get; set; } = false; // Test ortamında ignore et
}