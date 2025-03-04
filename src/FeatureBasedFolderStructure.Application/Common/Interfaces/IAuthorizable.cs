using System.Security.Claims;

namespace FeatureBasedFolderStructure.Application.Common.Interfaces;

public interface IAuthorizable 
{
    Task<bool> AuthorizeAsync(ClaimsPrincipal user);
}