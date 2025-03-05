using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Application.Requests;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<BaseResponse<ProductListDto>>
{
    public PageRequest PageRequest { get; set; } = new();
}