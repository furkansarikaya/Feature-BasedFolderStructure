namespace FeatureBasedFolderStructure.Domain.Common.Attributes;

/// <summary>
/// Configuration değerlerine bağlı olarak conditional service registration için kullanılır.
/// Feature flag mantığıyla service'leri açıp kapatabilirsiniz.
/// </summary>
[AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
public class ConditionalServiceAttribute(string configurationKey, string expectedValue) : Attribute
{
    public string ConfigurationKey { get; } = configurationKey;
    public string ExpectedValue { get; } = expectedValue;
}