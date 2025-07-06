using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Models.Requests;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.AspNetCore.ResponseWrapper.Models.Paging;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;

public sealed record GetProductsQuery(PageRequest PageRequest) : IRequest<PagedResult<ProductDto>>;

internal class GetProductsQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetProductsQuery, PagedResult<ProductDto>>
{
    public async Task<PagedResult<ProductDto>> HandleAsync(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var productRepository = unitOfWork.GetRepository<Product, int>();
        var products = await productRepository.GetPagedAsync(
            pageIndex: request.PageRequest.Page,
            pageSize: request.PageRequest.PageSize,
            orderBy: p => p.OrderBy(product => product.Name),
            cancellationToken: cancellationToken);
        return mapper.Map<PagedResult<ProductDto>>(products);
    }
}