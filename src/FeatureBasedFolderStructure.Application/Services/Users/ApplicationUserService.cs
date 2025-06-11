using System.Net;
using FeatureBasedFolderStructure.Application.Common.Models;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Application.Services.Users;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 20)]
public class ApplicationUserService(
    IApplicationUserRepository userRepository,
    ILogger<ApplicationUserService> logger)
    : IApplicationUserService
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public async Task<BaseResponse<ApplicationUser>> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            return user == null ? BaseResponse<ApplicationUser>.NotFound("Kullanıcı bulunamadı.") : BaseResponse<ApplicationUser>.SuccessResult(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı getirme hatası. {UserId}", id);
            return BaseResponse<ApplicationUser>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı getirme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<ApplicationUser>> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByEmailAsync(email, cancellationToken);
            return user == null ? BaseResponse<ApplicationUser>.NotFound("Kullanıcı bulunamadı.") : BaseResponse<ApplicationUser>.SuccessResult(user);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "E-posta ile kullanıcı getirme hatası. {Email}", email);
            return BaseResponse<ApplicationUser>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı getirme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<IEnumerable<ApplicationUser>>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await userRepository.GetAllAsync(cancellationToken: cancellationToken);
            return BaseResponse<IEnumerable<ApplicationUser>>.SuccessResult(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Tüm kullanıcıları getirme hatası.");
            return BaseResponse<IEnumerable<ApplicationUser>>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcıları getirme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<IEnumerable<ApplicationUser>>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await userRepository.GetByStatusAsync(status, cancellationToken);
            return BaseResponse<IEnumerable<ApplicationUser>>.SuccessResult(users);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Duruma göre kullanıcı getirme hatası. {Status}", status);
            return BaseResponse<IEnumerable<ApplicationUser>>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcıları getirme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<Guid>> CreateAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailExists = await userRepository.EmailExistsAsync(user.Email, cancellationToken);
            if (emailExists)
                return BaseResponse<Guid>.ErrorResult("İşlem başarısız", ["Bu e-posta adresi zaten kullanımda."]);

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            var result = await userRepository.AddAsync(user, cancellationToken);
            return result.Id == Guid.Empty ? BaseResponse<Guid>.ErrorResult("Kullanıcı oluşturulamadı", ["Kayıt işlemi başarısız."]) : BaseResponse<Guid>.SuccessResult(user.Id, HttpStatusCode.Created);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı oluşturma hatası. {Email}", user.Email);
            return BaseResponse<Guid>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı oluşturma işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<bool>> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await userRepository.GetByIdAsync(user.Id, cancellationToken);
            if (existingUser == null)
                return BaseResponse<bool>.NotFound("Güncellenecek kullanıcı bulunamadı.");

            user.PasswordHash = existingUser.PasswordHash;
            await userRepository.UpdateAsync(user, cancellationToken);
            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı güncelleme hatası. {UserId}", user.Id);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı güncelleme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<bool>> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return BaseResponse<bool>.NotFound("Silinecek kullanıcı bulunamadı.");

            await userRepository.DeleteAsync(user, cancellationToken);
            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı silme hatası. {UserId}", id);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı silme işlemi başarısız oldu."]);
        }
    }

    public BaseResponse<bool> VerifyPassword(ApplicationUser user, string password)
    {
        try
        {
            if (user == null)
                return BaseResponse<bool>.NotFound("Kullanıcı bulunamadı.");

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificationResult == PasswordVerificationResult.Failed)
                return BaseResponse<bool>.ErrorResult("Doğrulama başarısız", ["Şifre yanlış."]);

            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Şifre doğrulama hatası. {UserId}", user?.Id);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Şifre doğrulama işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<bool>> ChangePasswordByForgetPasswordAsync(Guid id, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return BaseResponse<bool>.NotFound("Kullanıcı bulunamadı.");
            
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await userRepository.UpdateAsync(user, cancellationToken);

            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Şifre değiştirme hatası. {UserId}", id);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Şifre değiştirme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<bool>> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return BaseResponse<bool>.NotFound("Kullanıcı bulunamadı.");

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
                return BaseResponse<bool>.ErrorResult("Şifre değiştirilemedi", ["Mevcut şifre yanlış."]);

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await userRepository.UpdateAsync(user, cancellationToken);

            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Şifre değiştirme hatası. {UserId}", id);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Şifre değiştirme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<bool>> LockUserAsync(Guid id, DateTime lockoutEnd, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return BaseResponse<bool>.NotFound("Kullanıcı bulunamadı.");

            user.LockoutEnd = lockoutEnd;
            await userRepository.UpdateAsync(user, cancellationToken);
            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı kilitleme hatası. {UserId}", id);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı kilitleme işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<bool>> UnlockUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return BaseResponse<bool>.NotFound("Kullanıcı bulunamadı.");

            user.LockoutEnd = null;
            await userRepository.UpdateAsync(user, cancellationToken);
            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı kilit açma hatası. {UserId}", id);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı kilit açma işlemi başarısız oldu."]);
        }
    }

    public async Task<BaseResponse<bool>> ChangeUserStatusAsync(Guid id, UserStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await userRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                return BaseResponse<bool>.NotFound("Kullanıcı bulunamadı.");

            user.Status = status;
            await userRepository.UpdateAsync(user, cancellationToken);
            return BaseResponse<bool>.SuccessResult(true);
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Kullanıcı durumu değiştirme hatası. {UserId}, {Status}", id, status);
            return BaseResponse<bool>.ErrorResult("İşlem sırasında hata oluştu", ["Kullanıcı durumu değiştirme işlemi başarısız oldu."]);
        }
    }
}