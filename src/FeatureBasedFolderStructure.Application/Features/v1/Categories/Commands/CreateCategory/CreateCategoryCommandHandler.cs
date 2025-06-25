using AutoMapper;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FS.EntityFramework.Library.UnitOfWorks;
using MediatR;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.CreateCategory;

public class CreateCategoryCommandHandler(
    IUnitOfWork unitOfWork,
    ILogger<CreateCategoryCommandHandler> logger,
    IMapper mapper)
    : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var entity = new Category
        {
            Name = request.Name,
            Description = request.Description
        };
        var categoryRepository = unitOfWork.GetRepository<Category, int>();
        await categoryRepository.AddAsync(entity, cancellationToken: cancellationToken);
        await unitOfWork.SaveChangesAsync(cancellationToken);
        logger.LogInformation("Created Category {CategoryId}", entity.Id);

        return mapper.Map<CategoryDto>(entity);
    }
}