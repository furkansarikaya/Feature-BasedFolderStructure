using System.Security.Claims;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using Microsoft.AspNetCore.Http;

namespace FeatureBasedFolderStructure.Infrastructure.Services;

public class CurrentUserService(IHttpContextAccessor httpContextAccessor) : ICurrentUserService
{
    public string UserId => httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "System";
}