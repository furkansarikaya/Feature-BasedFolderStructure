using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 100)]
public class AuthBusinessRules(IApplicationUserService applicationUserService)
{
    public async Task EmailCanNotBeDuplicatedWhenRegistered(string email)
    {
        var user = await applicationUserService.GetByEmailAsync(email);
        if (user != null && user.Id != Guid.Empty) throw new BusinessException("Mail already exists");
    }
}