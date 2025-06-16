using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
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
        if (!applicationUser.Success)
            throw new NotFoundException(nameof(applicationUser.Data), request.Email);

        if (applicationUser.Data.Status != UserStatus.Active)
        {
            switch (applicationUser.Data.Status)
            {
                case UserStatus.Inactive:
                    throw new NotFoundException(nameof(applicationUser.Data), request.Email);
                case UserStatus.Locked when applicationUser.Data.LockoutEnd != null && applicationUser.Data.LockoutEnd > DateTime.Now:
                    throw new BusinessException("Kullanıcı hesabı kilitli.");
                case UserStatus.Suspended:
                    throw new BusinessException("Kullanıcı hesabı askıya alındı.");
                case UserStatus.PendingActivation:
                    throw new BusinessException("Kullanıcı hesabı aktifleştirilmemiş.");
            }
        }
        
        var passwordVerification = applicationUserService.VerifyPassword(applicationUser.Data, request.Password);
        if (!passwordVerification.Success)
        {
            applicationUser.Data.AccessFailedCount++;
            if (applicationUser.Data.AccessFailedCount >= 3)
            {
                applicationUser.Data.Status = UserStatus.Locked;
                applicationUser.Data.LockoutEnd = DateTime.Now.AddHours(1);
            }
            await applicationUserService.UpdateAsync(applicationUser.Data, cancellationToken);
            
            throw new BusinessException("Kullanıcı adı veya şifre hatalı.");
        }

        if (applicationUser.Data.Status == UserStatus.Locked)
        {
            applicationUser.Data.Status = UserStatus.Active;
            applicationUser.Data.LockoutEnd = null;
            applicationUser.Data.AccessFailedCount = 0;
            await applicationUserService.UpdateAsync(applicationUser.Data, cancellationToken);
        }

        var accessToken = await tokenService.GenerateTokenAsync(applicationUser.Data.Id, TokenType.AccessToken);
        var refreshToken = await tokenService.GenerateTokenAsync(applicationUser.Data.Id, TokenType.RefreshToken);
        return new LoginDto(accessToken.Token, accessToken.ExpiryDate, refreshToken.Token, refreshToken.ExpiryDate);
    }
}