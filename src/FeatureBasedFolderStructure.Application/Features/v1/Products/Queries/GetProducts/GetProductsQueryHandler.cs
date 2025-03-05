using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;

public class GetProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductsQuery, BaseResponse<ProductListDto>>
{
    public async Task<BaseResponse<ProductListDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetListAsync(
            orderBy: p => p.OrderBy(product => product.Name),
            index: request.PageRequest.Page,
            size: request.PageRequest.PageSize,
            cancellationToken: cancellationToken);
        return BaseResponse<ProductListDto>.SuccessResult(mapper.Map<ProductListDto>(products));
    }
}