using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.Categories.Commands.CreateCategory;
using FeatureBasedFolderStructure.Application.Features.Categories.Commands.DeleteCategory;
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
        var response = await Mediator.Send(new GetCategoriesQuery(), cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetCategory(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetCategoryDetailQuery { Id = id }, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateCategoryCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateCategoryCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            var errorResponse = BaseResponse<object>.ErrorResult("Id in request does not match Id in command", ["Id in request does not match Id in command"]);
            return StatusCode((int)errorResponse.StatusCode, errorResponse);
        }

        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }
    
    [HttpDelete("{id}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new DeleteCategoryCommand(id), cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }
}