using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Common.UnitOfWork;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.ValueObjects.Users;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Register;

public class RegisterCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService, AuthBusinessRules authBusinessRules, IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, RegisterDto>
{
    public async Task<RegisterDto> Handle(RegisterCommand request, CancellationToken cancellationToken)
    {
        var applicationUserId = await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await authBusinessRules.EmailCanNotBeDuplicatedWhenRegistered(request.Email);
            var customerRole = await unitOfWork.RoleRepository.GetByNameAsync("CUSTOMER");
            if (customerRole == null)
            {
                customerRole = new Role { Name = "Customer", NormalizedName = "CUSTOMER" };
                await unitOfWork.RoleRepository.AddAsync(customerRole, cancellationToken);
            }

            var newUser = CreateUser(request, customerRole);
            var result = await applicationUserService.CreateAsync(newUser, request.Password, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return result;
        });

        return await GenerateTokens(applicationUserId);
    }

    private static ApplicationUser CreateUser(RegisterCommand request, Role customerRole)
    {
        return new ApplicationUser
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
    }

    private async Task<RegisterDto> GenerateTokens(Guid applicationUserId)
    {
        var emailConfirmation = await tokenService.GenerateTokenAsync(applicationUserId, TokenType.EmailConfirmation);
        var accessToken = await tokenService.GenerateTokenAsync(applicationUserId, TokenType.AccessToken);
        var refreshToken = await tokenService.GenerateTokenAsync(applicationUserId, TokenType.RefreshToken);

        return new RegisterDto(accessToken.Token, accessToken.ExpiryDate, refreshToken.Token, refreshToken.ExpiryDate);
    }
}