using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.DeleteProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.UpdateProduct;
using FeatureBasedFolderStructure.Application.Features.v1.Products.DTOs;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProductDetail;
using FeatureBasedFolderStructure.Application.Features.v1.Products.Queries.GetProducts;
using FeatureBasedFolderStructure.Application.Requests;
using FeatureBasedFolderStructure.Domain.Common.Paging;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class ProductsController : BaseController
{
    [HttpGet]
    public async Task<PagedResult<ProductDto>> GetProducts([FromQuery] PageRequest pageRequest, CancellationToken cancellationToken)
    {
        var result = await Mediator.Send(new GetProductsQuery { PageRequest = pageRequest }, cancellationToken);
        return result;
    }

    [HttpGet("{id:int}")]
    public async Task<ProductDto> GetProduct(int id, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetProductDetailQuery { Id = id }, cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var product = await Mediator.Send(command, cancellationToken);
        return CreatedAtAction(nameof(GetProduct), new { id = product.Id }, product);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            throw new ArgumentException($"Product ID mismatch. The ID in the URL must match the ID in the command. Please check your request. The ID in the URL is: {id}, the ID in the command is: {command.Id}");
        }

        await Mediator.Send(command, cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        await Mediator.Send(new DeleteProductCommand(id), cancellationToken);
        return NoContent();
    }
}