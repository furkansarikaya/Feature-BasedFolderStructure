using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.DeleteProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.UpdateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProductDetail;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;
using FeatureBasedFolderStructure.Application.Requests;
using FeatureBasedFolderStructure.Domain.Common.Paging;
using FS.AspNetCore.ResponseWrapper.Models;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class ProductsController : BaseController
{
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<CleanPagedResult<ProductDto>>))]
    public async Task<PagedResult<ProductDto>> GetProducts([FromQuery] PageRequest pageRequest, CancellationToken cancellationToken)
    {
        var result = await Mediator.SendAsync(new GetProductsQuery(pageRequest), cancellationToken);
        return result;
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(ApiResponse<ProductDto>))]
    public async Task<ProductDto> GetProduct(int id, CancellationToken cancellationToken)
    {
        return await Mediator.SendAsync(new GetProductDetailQuery(id), cancellationToken);
    }

    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created, Type = typeof(ApiResponse<ProductDto>))]
    public async Task<ActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await Mediator.SendAsync(command, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            throw new ArgumentException($"Product ID mismatch. The ID in the URL must match the ID in the command. Please check your request. The ID in the URL is: {id}, the ID in the command is: {command.Id}");
        }

        await Mediator.SendAsync(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.SendAsync(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}