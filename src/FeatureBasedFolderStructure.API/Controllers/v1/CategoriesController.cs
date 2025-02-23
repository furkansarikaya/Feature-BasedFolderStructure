using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Features.Categories.Commands.CreateCategory;
using FeatureBasedFolderStructure.Application.Features.Categories.Commands.UpdateCategory;
using FeatureBasedFolderStructure.Application.Features.Categories.DTOs;
using FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategories;
using FeatureBasedFolderStructure.Application.Features.Categories.Queries.GetCategoryDetail;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class CategoriesController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetCategories(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetCategoriesQuery(), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetCategoryDetailQuery { Id = id }, cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest();

        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}