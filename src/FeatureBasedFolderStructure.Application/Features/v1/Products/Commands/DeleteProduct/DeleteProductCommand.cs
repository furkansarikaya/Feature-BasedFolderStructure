using FeatureBasedFolderStructure.Application.Common.Models;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(int Id) : IRequest<BaseResponse<Unit>>;