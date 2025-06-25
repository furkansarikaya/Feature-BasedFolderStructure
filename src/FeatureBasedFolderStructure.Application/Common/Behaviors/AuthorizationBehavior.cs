using System.Reflection;
using FeatureBasedFolderStructure.Application.Common.Attributes;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Common.Behaviors;

public class AuthorizationBehavior<TRequest, TResponse>(
    ILogger<AuthorizationBehavior<TRequest, TResponse>> logger,
    IHttpContextAccessor httpContextAccessor)
    : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{
    public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken cancellationToken)
    {
        var requestType = request.GetType();

        // RequiresClaim attribute'unu kontrol et
        var requiresClaimAttributes = requestType.GetCustomAttributes<RequiresClaimAttribute>(true);

        // Eğer attribute yoksa, yetkilendirme kontrolü gerekmez
        if (!requiresClaimAttributes.Any())
        {
            return await next();
        }

        logger.LogInformation("Authorization kontrolü yapılıyor: {RequestType}", requestType.Name);

        // Kullanıcı oturum açmış mı?
        var user = httpContextAccessor.HttpContext?.User;
        if (user?.Identity?.IsAuthenticated != true)
        {
            logger.LogWarning("Yetkisiz erişim denemesi: Kullanıcı oturum açmamış");
            throw new UnauthorizedAccessException("Bu işlemi gerçekleştirmek için oturum açmalısınız.");
        }

        // Tüm gerekli claim'leri kontrol et
        foreach (var attribute in requiresClaimAttributes)
        {
            // Claim ve değer kontrolü
            if (attribute.RequireAllValues)
            {
                // Tüm değerlerin mevcut olması gerekiyorsa
                if (!attribute.ClaimValues.All(value =>
                        user.HasClaim(c => c.Type == attribute.ClaimType && c.Value == value)))
                {
                    logger.LogWarning("Erişim reddedildi: Kullanıcı gerekli tüm claim değerlerine sahip değil. Claim: {ClaimType}", attribute.ClaimType);
                    throw new ForbiddenAccessException($"Bu işlemi gerçekleştirmek için gereken '{attribute.ClaimType}' yetkisine sahip değilsiniz.");
                }
            }
            else
            {
                // En az bir değerin mevcut olması yeterli
                if (!attribute.ClaimValues.Any(value =>
                        user.HasClaim(c => c.Type == attribute.ClaimType && c.Value == value)))
                {
                    logger.LogWarning("Erişim reddedildi: Kullanıcı gerekli claim değerlerine sahip değil. Claim: {ClaimType}", attribute.ClaimType);
                    throw new ForbiddenAccessException($"Bu işlemi gerçekleştirmek için gereken '{attribute.ClaimType}' yetkisine sahip değilsiniz.");
                }
            }
        }

        logger.LogInformation("Yetkilendirme başarılı: {RequestType}", requestType.Name);
        return await next();
    }
}
