using FeatureBasedFolderStructure.Domain.Entities;
using FeatureBasedFolderStructure.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(ICategoryRepository categoryRepository, 
    ILogger<CreateCategoryCommandHandler> logger)
    : IRequestHandler<CreateCategoryCommand, int>
{
    public async Task<int> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Name = request.Name,
            Description = request.Description
        };

        await categoryRepository.AddAsync(entity, cancellationToken);

        logger.LogInformation("Created Category {CategoryId}", entity.Id);

        return entity.Id;
    }
}