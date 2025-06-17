namespace FeatureBasedFolderStructure.API.Filters;

/// <summary>
/// Bu attribute ile işaretlenen action'larda automatic API response wrapping SKIP edilir.
/// Örnek kullanım senaryoları:
/// - File download endpoint'leri
/// - Redirect action'ları  
/// - Custom response format gerektiren endpoint'ler
/// - Third-party integration endpoint'leri (webhook'lar gibi)
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class SkipApiResponseWrapperAttribute : Attribute
{
    public string Reason { get; set; } = "";
    
    public SkipApiResponseWrapperAttribute(string reason = "")
    {
        Reason = reason;
    }
}