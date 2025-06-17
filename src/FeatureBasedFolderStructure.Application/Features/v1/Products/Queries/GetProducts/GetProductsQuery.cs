using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Application.Requests;
using FeatureBasedFolderStructure.Domain.Common.Paging;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<PagedResult<ProductDto>>
{
    public PageRequest PageRequest { get; set; } = new();
}