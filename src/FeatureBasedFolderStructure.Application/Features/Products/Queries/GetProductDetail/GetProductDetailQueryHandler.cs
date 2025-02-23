using AutoMapper;
using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Features.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Queries.GetProductDetail;

public class GetProductDetailQueryHandler(
    IProductRepository productRepository,
    IMapper mapper) : IRequestHandler<GetProductDetailQuery, ProductDto>
{
    public async Task<ProductDto> Handle(GetProductDetailQuery request, CancellationToken cancellationToken)
    {
        var product = await productRepository.GetByIdAsync(request.Id, cancellationToken, true);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);

        return mapper.Map<ProductDto>(product);
    }
}