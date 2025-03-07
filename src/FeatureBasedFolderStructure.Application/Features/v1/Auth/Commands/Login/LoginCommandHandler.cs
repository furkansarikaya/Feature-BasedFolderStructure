using FeatureBasedFolderStructure.Application.Common.Interfaces;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Features.v1.Auth.DTOs;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using MediatR;

namespace FeatureBasedFolderStructure.Application.Features.v1.Auth.Commands.Login;

public class LoginCommandHandler(IApplicationUserService applicationUserService,ITokenService tokenService) : IRequestHandler<LoginCommand, BaseResponse<LoginDto>>
{
    public async Task<BaseResponse<LoginDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
    {
        var applicationUser = await applicationUserService.GetByEmailAsync(request.Email, cancellationToken);
        if (!applicationUser.Success)
            return BaseResponse<LoginDto>.NotFound(applicationUser.Message);

        if (applicationUser.Data.Status != UserStatus.Active)
        {
            switch (applicationUser.Data.Status)
            {
                case UserStatus.Inactive:
                    return BaseResponse<LoginDto>.NotFound("Kullanıcı bulunamadı.");
                case UserStatus.Locked:
                    return BaseResponse<LoginDto>.ErrorResult("Kullanıcı hesabı kilitli.", ["Kullanıcı hesabı kilitli."]);
                case UserStatus.Suspended:
                    return BaseResponse<LoginDto>.ErrorResult("Kullanıcı hesabı askıya alındı.", ["Kullanıcı hesabı askıya alındı."]);
                case UserStatus.PendingActivation:
                    return BaseResponse<LoginDto>.ErrorResult("Kullanıcı hesabı aktifleştirilmemiş.", ["Kullanıcı hesabı aktifleştirilmemiş."]);
            }
        }
        
        var passwordVerification = applicationUserService.VerifyPassword(applicationUser.Data, request.Password);
        if (!passwordVerification.Success)
            return BaseResponse<LoginDto>.ErrorResult("Kullanıcı adı veya şifre hatalı.", ["Kullanıcı adı veya şifre hatalı."]);

        var accessToken = await tokenService.GenerateTokenAsync(applicationUser.Data.Id, TokenType.AccessToken);
        var refreshToken = await tokenService.GenerateTokenAsync(applicationUser.Data.Id, TokenType.RefreshToken);
        return BaseResponse<LoginDto>.SuccessResult(new LoginDto(accessToken.Token, accessToken.ExpiryDate, refreshToken.Token, refreshToken.ExpiryDate));
    }
}