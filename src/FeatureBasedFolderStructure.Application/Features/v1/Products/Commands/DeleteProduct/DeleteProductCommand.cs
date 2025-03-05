using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<BaseResponse<Unit>>;