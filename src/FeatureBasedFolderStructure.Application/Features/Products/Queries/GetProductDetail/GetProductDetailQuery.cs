using FeatureBasedFolderStructure.Application.Features.Products.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Queries.GetProductDetail;

public class GetProductDetailQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}