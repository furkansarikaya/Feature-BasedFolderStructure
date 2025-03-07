using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;

public class AuthBusinessRules(IApplicationUserService applicationUserService, IRoleRepository roleRepository)
{
    public async Task EmailCanNotBeDuplicatedWhenRegistered(string email)
    {
        var user = await applicationUserService.GetByEmailAsync(email);
        if (user.Success) throw new BusinessException("Mail already exists");
    }

    public async Task<Role> CustomerRoleMustBeExist()
    {
        var customerRole = await roleRepository.GetByNameAsync("CUSTOMER");
        if (customerRole != null) return customerRole;
        customerRole = new Role { Name = "Customer", NormalizedName = "CUSTOMER" };
        await roleRepository.AddAsync(customerRole);
        return customerRole;
    }
}