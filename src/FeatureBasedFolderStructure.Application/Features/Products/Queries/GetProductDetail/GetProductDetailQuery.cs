using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.Products.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Queries.GetProductDetail;

public class GetProductDetailQuery : IRequest<BaseResponse<ProductDto>>
{
    public int Id { get; set; }
}