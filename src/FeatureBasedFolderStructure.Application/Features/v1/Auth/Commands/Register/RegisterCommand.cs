using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Interfaces.Users;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.Rules;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.ValueObjects.Users;
using FS.EntityFramework.Library.UnitOfWorks;
using FS.Mediator.Features.RequestHandling.Core;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Register;

public sealed record RegisterCommand(string Email, string Password, string FirstName, string LastName) : IRequest<RegisterDto>;

internal class RegisterCommandHandler(IApplicationUserService applicationUserService, ITokenService tokenService, AuthBusinessRules authBusinessRules, IUnitOfWork unitOfWork) : IRequestHandler<RegisterCommand, RegisterDto>
{
    public async Task<RegisterDto> HandleAsync(RegisterCommand request, CancellationToken cancellationToken)
    {
        var applicationUserId = await unitOfWork.ExecuteInTransactionAsync(async () =>
        {
            await authBusinessRules.EmailCanNotBeDuplicatedWhenRegistered(request.Email);
            var roleRepository = unitOfWork.GetRepository<Role, int>();
            var customerRole = await roleRepository.FirstOrDefaultAsync(r => r.NormalizedName == "CUSTOMER", cancellationToken: cancellationToken);
            if (customerRole == null)
            {
                customerRole = new Role { Name = "Customer", NormalizedName = "CUSTOMER" };
                await roleRepository.AddAsync(customerRole, cancellationToken: cancellationToken);
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
        _ = await tokenService.GenerateTokenAsync(applicationUserId, TokenType.EmailConfirmation);
        var accessToken = await tokenService.GenerateTokenAsync(applicationUserId, TokenType.AccessToken);
        var refreshToken = await tokenService.GenerateTokenAsync(applicationUserId, TokenType.RefreshToken);

        return new RegisterDto(accessToken.Token, accessToken.ExpiryDate, refreshToken.Token, refreshToken.ExpiryDate);
    }
}