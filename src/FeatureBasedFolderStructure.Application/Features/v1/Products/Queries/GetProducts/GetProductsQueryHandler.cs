using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;

public class GetProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductsQuery, BaseResponse<ProductListDto>>
{
    public async Task<BaseResponse<ProductListDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = productRepository.GetPagedAsync(
            pageIndex: request.PageRequest.Page,
            pageSize: request.PageRequest.PageSize,
            orderBy: p => p.OrderBy(product => product.Name),
            cancellationToken: cancellationToken);
        return BaseResponse<ProductListDto>.SuccessResult(mapper.Map<ProductListDto>(products));
    }
}