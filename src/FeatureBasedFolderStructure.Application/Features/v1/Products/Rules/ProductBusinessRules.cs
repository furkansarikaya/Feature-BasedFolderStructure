using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Domain.Interfaces.Catalogs;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Rules;

public class ProductBusinessRules(ICategoryRepository categoryRepository)
{
    public async Task CheckIfCategoryExists(int categoryId, CancellationToken cancellationToken)
    {
        var category = await categoryRepository.GetByIdAsync(categoryId, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(category), categoryId);
    }
}