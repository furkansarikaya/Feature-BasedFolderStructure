using Bogus;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateFakeProducts;

public sealed record CreateFakeProductsCommand(int Count) : IRequest<Unit>;

internal class CreateFakeProductsCommandHandler(IUnitOfWork unitOfWork) : IRequestHandler<CreateFakeProductsCommand, Unit>
{
    public async Task<Unit> HandleAsync(CreateFakeProductsCommand request, CancellationToken cancellationToken)
    {
        for (var i = 0; i < request.Count; i++)
        {
            var fakerProduct = new Faker<Product>().RuleFor(p => p.Name, f => f.Commerce.ProductName())
                .RuleFor(p => p.Price, f => f.Random.Decimal(1, 100))
                .RuleFor(p => p.Description, f => f.Commerce.ProductDescription())
                .RuleFor(p => p.CategoryId, f => f.Random.Int(7, 10));
            
            var product = fakerProduct.Generate();
            product.UpdatePrice(product.Price, "TRY");
            var productRepository = unitOfWork.GetRepository<Product, int>();
            await productRepository.AddAsync(product, cancellationToken: cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
        }
        return Unit.Value;
    }
}