using System.Security.Claims;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FS.AutoServiceDiscovery.Extensions.Attributes;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Features.Common.Services;

[ServiceRegistration(ServiceLifetime.Scoped, Order = -1)]
public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
}