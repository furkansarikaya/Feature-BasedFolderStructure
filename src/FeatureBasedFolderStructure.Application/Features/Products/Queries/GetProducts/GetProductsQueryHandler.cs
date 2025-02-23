using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Queries.GetProducts;

public class GetProductsQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductsQuery, List<ProductDto>>
{
    public async Task<List<ProductDto>> Handle(GetProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await productRepository.GetAllAsync(cancellationToken);
        return mapper.Map<List<ProductDto>>(products.OrderBy(p => p.Name).ToList());
    }
}