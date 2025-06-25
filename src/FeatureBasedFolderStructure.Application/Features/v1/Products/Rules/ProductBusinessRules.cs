using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using FS.EntityFramework.Library.UnitOfWorks;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Rules;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 100)]
public class ProductBusinessRules(IUnitOfWork unitOfWork)
{
    public async Task CheckIfCategoryExists(int categoryId, CancellationToken cancellationToken)
    {
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        var category = await categoryRepository.GetByIdAsync(categoryId, true, cancellationToken);
        if (category == null)
            throw new NotFoundException(nameof(category), categoryId);
    }
}