using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Features.Products.Commands.CreateProduct;
using FeatureBasedFolderStructure.Application.Features.Products.Commands.UpdateProduct;
using FeatureBasedFolderStructure.Application.Features.Products.DTOs;
using FeatureBasedFolderStructure.Application.Features.Products.Queries.GetProductDetail;
using FeatureBasedFolderStructure.Application.Features.Products.Queries.GetProducts;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.v1;

[ApiVersion("1.0")]
public class ProductsController : BaseController
{
    [HttpGet]
    public async Task<ActionResult<List<ProductDto>>> GetProducts(CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetProductsQuery(), cancellationToken);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken cancellationToken)
    {
        return await Mediator.Send(new GetProductDetailQuery { Id = id }, cancellationToken);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        return await Mediator.Send(command, cancellationToken);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
            return BadRequest();

        await Mediator.Send(command, cancellationToken);

        return NoContent();
    }
}