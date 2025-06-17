using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;

public class LoginCommandHandler(IApplicationUserService applicationUserService,ITokenService tokenService) : IRequestHandler<LoginCommand, LoginDto>
{
    public async Task<LoginDto> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var applicationUser = await applicationUserService.GetByEmailAsync(request.Email, cancellationToken);
        if (applicationUser.Status != UserStatus.Active)
        {
            switch (applicationUser.Status)
            {
                case UserStatus.Inactive:
                    throw new NotFoundException(nameof(applicationUser), request.Email);
                case UserStatus.Locked when applicationUser.LockoutEnd != null && applicationUser.LockoutEnd > DateTime.Now:
                    throw new BusinessException("Kullanıcı hesabı kilitli.");
                case UserStatus.Suspended:
                    throw new BusinessException("Kullanıcı hesabı askıya alındı.");
                case UserStatus.PendingActivation:
                    throw new BusinessException("Kullanıcı hesabı aktifleştirilmemiş.");
            }
        }
        
        var passwordVerification = applicationUserService.VerifyPassword(applicationUser, request.Password);
        if (!passwordVerification)
        {
            applicationUser.AccessFailedCount++;
            if (applicationUser.AccessFailedCount >= 3)
            {
                applicationUser.Status = UserStatus.Locked;
                applicationUser.LockoutEnd = DateTime.Now.AddHours(1);
            }
            await applicationUserService.UpdateAsync(applicationUser, cancellationToken);
            
            throw new BusinessException("Kullanıcı adı veya şifre hatalı.");
        }

        if (applicationUser.Status == UserStatus.Locked)
        {
            applicationUser.Status = UserStatus.Active;
            applicationUser.LockoutEnd = null;
            applicationUser.AccessFailedCount = 0;
            await applicationUserService.UpdateAsync(applicationUser, cancellationToken);
        }

        var accessToken = await tokenService.GenerateTokenAsync(applicationUser.Id, TokenType.AccessToken);
        var refreshToken = await tokenService.GenerateTokenAsync(applicationUser.Id, TokenType.RefreshToken);
        return new LoginDto(accessToken.Token, accessToken.ExpiryDate, refreshToken.Token, refreshToken.ExpiryDate);
    }
}