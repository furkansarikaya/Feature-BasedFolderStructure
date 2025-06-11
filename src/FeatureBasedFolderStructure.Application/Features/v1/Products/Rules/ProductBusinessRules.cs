using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Rules;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 100)]
public class ProductBusinessRules(ICategoryRepository categoryRepository)
{
    public async Task CheckIfCategoryExists(int categoryId, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(category), categoryId);
    }
}