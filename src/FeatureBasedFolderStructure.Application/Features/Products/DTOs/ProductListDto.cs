using FeatureBasedFolderStructure.Domain.Common.Paging;

namespace FeatureBasedFolderStructure.Application.Features.Products.DTOs;

public class ProductListDto : BasePageableModel
{
    public IList<ProductDto> Items { get; set; }
}