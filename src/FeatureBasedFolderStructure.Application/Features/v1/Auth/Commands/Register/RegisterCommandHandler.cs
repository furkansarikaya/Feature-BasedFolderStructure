using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.ValueObjects.Users;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Register;

public class RegisterCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService, AuthBusinessRules authBusinessRules, IRoleRepository roleRepository) : IRequestHandler<RegisterCommand, BaseResponse<RegisterDto>>
{
    public async Task<BaseResponse<RegisterDto>> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        await authBusinessRules.EmailCanNotBeDuplicatedWhenRegistered(request.Email);

        var customerRole = await authBusinessRules.CustomerRoleMustBeExist();

        var newUser = new ApplicationUser
        {
            Email = request.Email,
            FullName = FullName.Create(request.FirstName, request.LastName),
            EmailConfirmed = false,
            Status = UserStatus.PendingActivation,
            UserRoles = new List<UserRole>
            {
                new()
                {
                    RoleId = customerRole.Id
                }
            }
        };

        var result = await applicationUserService.CreateAsync(newUser, request.Password, cancellationToken);
        if (!result.Success)
            return BaseResponse<RegisterDto>.ErrorResult(result.Message, result.Errors);
        var emailConfirmation = await tokenService.GenerateTokenAsync(result.Data, TokenType.EmailConfirmation);
        var accessToken = await tokenService.GenerateTokenAsync(result.Data, TokenType.AccessToken);
        var refreshToken = await tokenService.GenerateTokenAsync(result.Data, TokenType.RefreshToken);
        return BaseResponse<RegisterDto>.SuccessResult(new RegisterDto(accessToken.Token, accessToken.ExpiryDate, refreshToken.Token, refreshToken.ExpiryDate));
    }
}