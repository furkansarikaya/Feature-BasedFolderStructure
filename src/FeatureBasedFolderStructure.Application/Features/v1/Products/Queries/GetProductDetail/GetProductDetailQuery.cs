using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProductDetail;

public class GetProductDetailQuery : IRequest<ProductDto>
{
    public int Id { get; set; }
}