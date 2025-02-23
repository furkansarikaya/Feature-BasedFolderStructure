using FeatureBasedFolderStructure.API.Controllers.Base;
using FeatureBasedFolderStructure.Application.Common.Models;
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
        var response = await Mediator.Send(new GetProductsQuery(), cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetProduct(int id, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(new GetProductDetailQuery { Id = id }, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPost]
    public async Task<ActionResult<int>> Create(CreateProductCommand command, CancellationToken cancellationToken)
    {
        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }

    [HttpPut("{id}")]
    public async Task<ActionResult> Update(int id, UpdateProductCommand command, CancellationToken cancellationToken)
    {
        if (id != command.Id)
        {
            var errorResponse = BaseResponse<object>.ErrorResult("Id in request does not match Id in command", ["Id in request does not match Id in command"]);
            return StatusCode((int)errorResponse.StatusCode, errorResponse);
        }

        var response = await Mediator.Send(command, cancellationToken);
        return StatusCode((int)response.StatusCode, response);
    }
}