namespace FeatureBasedFolderStructure.API.Filters;

/// <summary>
/// Special result type'ları mark etmek için marker interface.
/// Örnek: Custom export result, custom report result, etc.
/// Bu interface'i implement eden result'lar otomatik wrapping'den exempt olur.
/// </summary>
public interface ISpecialResult
{
    // Marker interface - implementation gerekmez
}