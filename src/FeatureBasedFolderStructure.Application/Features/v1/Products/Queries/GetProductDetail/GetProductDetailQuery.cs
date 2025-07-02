using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProductDetail;

public sealed record GetProductDetailQuery(int Id) : IRequest<ProductDto>;

internal class GetProductDetailQueryHandler(
    IUnitOfWork unitOfWork,
    IMapper mapper) : IRequestHandler<GetProductDetailQuery, ProductDto>
{
    public async Task<ProductDto> HandleAsync(GetProductDetailQuery request, CancellationToken cancellationToken)
    {
        var productRepository = unitOfWork.GetRepository<Product, int>();
        var product = await productRepository.GetByIdAsync(request.Id, true, cancellationToken);

        if (product == null)
            throw new NotFoundException(nameof(Product), request.Id);
        
        return mapper.Map<ProductDto>(product);
    }
}