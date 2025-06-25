using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.CreateCategory;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.DeleteCategory;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.Commands.UpdateCategory;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.DTOs;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategories;
using FeatureBasedFolderStructure.Application.Features.v1.Categories.Queries.GetCategoryDetail;
using FS.AspNetCore.ResponseWrapper.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class CategoriesController : BaseController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<List<CategoryDto>>))]
    public async Task<List<CategoryDto>> GetCategories(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetCategoriesQuery(), cancellationToken);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CategoryDto>))]
    public async Task<CategoryDto> GetCategory(int id, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetCategoryDetailQuery { Id = id }, cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<CategoryDto>))]
    public async Task<IActionResult> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var category = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetCategory), new { id = category.Id }, category);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(int id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            throw new ArgumentException($"Category ID mismatch. The ID in the URL must match the ID in the command. Please check your request. The ID in the URL is: {id}, the ID in the command is: {command.Id}");
        }

        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }
    
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}