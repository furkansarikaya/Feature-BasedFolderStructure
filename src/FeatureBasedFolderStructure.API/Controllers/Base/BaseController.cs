using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FeatureBasedFolderStructure.API.Controllers.Base;

[ApiController]
[Route("api/[controller]")]
public abstract class BaseController : ControllerBase
{
    private IMediator? _mediator;
    protected IMediator Mediator => _mediator ??= HttpContext.RequestServices.GetRequiredService<IMediator>();
}