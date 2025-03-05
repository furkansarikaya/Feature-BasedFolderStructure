using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProductDetail;

public class GetProductDetailQuery : IRequest<BaseResponse<ProductDto>>
{
    public int Id { get; set; }
}