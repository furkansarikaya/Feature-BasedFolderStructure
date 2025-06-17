namespace FeatureBasedFolderStructure.Domain.Common.Paging;

/// <summary>
/// Clean paged result - SADECE business data, pagination bilgileri YOK.
/// Bu structure transform edilmiş data için kullanılıyor.
/// </summary>
public class CleanPagedResult<T>
{
    /// <summary>
    /// Pure business data items - NO pagination metadata.
    /// Pagination bilgileri metadata'da olacak.
    /// </summary>
    public List<T> Items { get; set; } = new();
    
    // NO PAGE, NO PAGESIZE, NO TOTALPAGES, NO TOTALITEMS!
    // CLEAN SEPARATION ACHIEVED!
}