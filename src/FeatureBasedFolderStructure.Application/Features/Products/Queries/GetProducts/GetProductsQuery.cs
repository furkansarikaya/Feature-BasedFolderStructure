using FeatureBasedFolderStructure.Application.Features.Products.DTOs;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Queries.GetProducts;

public class GetProductsQuery : IRequest<List<ProductDto>>
{
}