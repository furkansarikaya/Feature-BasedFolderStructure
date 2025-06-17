using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Common.Paging;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;

public class GetProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetPagedAsync(
            pageIndex: request.PageRequest.Page,
            pageSize: request.PageRequest.PageSize,
            orderBy: p => p.OrderBy(product => product.Name),
            cancellationToken: cancellationToken);
        return mapper.Map<PagedResult<ProductDto>>(products);
    }
}