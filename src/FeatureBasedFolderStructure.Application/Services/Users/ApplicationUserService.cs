using FeatureBasedFolderStructure.Application.Common.Exceptions;
using FeatureBasedFolderStructure.Application.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Common.Attributes;
using FeatureBasedFolderStructure.Domain.Common.UnitOfWork;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Domain.Interfaces.Users;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Application.Services.Users;

[ServiceRegistration(ServiceLifetime.Scoped, Order = 20)]
public class ApplicationUserService(
    IUnitOfWork unitOfWork)
    : IApplicationUserService
{
    private readonly PasswordHasher<ApplicationUser> _passwordHasher = new();

    public async Task<ApplicationUser> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByIdAsync(id, cancellationToken);
            return user ?? throw new NotFoundException(nameof(ApplicationUser), id);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<ApplicationUser> GetUserWithRolesAndClaims(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetUserWithRolesAndClaims(id, cancellationToken);
            return user ?? throw new NotFoundException(nameof(ApplicationUser), id);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<ApplicationUser> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByEmailAsync(email, cancellationToken);
            return user ?? throw new NotFoundException(nameof(ApplicationUser), email);
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await unitOfWork.ApplicationUserRepository.GetAllAsync(cancellationToken: cancellationToken);
            return users;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<IEnumerable<ApplicationUser>> GetByStatusAsync(UserStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var users = await unitOfWork.ApplicationUserRepository.GetByStatusAsync(status, cancellationToken);
            return users;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<Guid> CreateAsync(ApplicationUser user, string password, CancellationToken cancellationToken = default)
    {
        try
        {
            var emailExists = await unitOfWork.ApplicationUserRepository.EmailExistsAsync(user.Email, cancellationToken);
            if (emailExists)
                throw new BusinessException("Bu e-posta adresi zaten kullanımda.");

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            var result = await unitOfWork.ApplicationUserRepository.AddAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return result.Id == Guid.Empty ? throw new BusinessException("Kullanıcı oluşturulamadı.") : user.Id;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<bool> UpdateAsync(ApplicationUser user, CancellationToken cancellationToken = default)
    {
        try
        {
            var existingUser = await unitOfWork.ApplicationUserRepository.GetByIdAsync(user.Id, cancellationToken);
            if (existingUser == null)
                throw new NotFoundException(nameof(ApplicationUser), user.Id);

            user.PasswordHash = existingUser.PasswordHash;
            await unitOfWork.ApplicationUserRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            await unitOfWork.ApplicationUserRepository.DeleteAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public bool VerifyPassword(ApplicationUser user, string password)
    {
        try
        {
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), "Kullanıcı bulunamadı.");

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, password);

            if (verificationResult == PasswordVerificationResult.Failed)
                throw new BusinessException("Şifre doğrulama başarısız.");

            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<bool> ChangePasswordByForgetPasswordAsync(Guid id, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);
            
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await unitOfWork.ApplicationUserRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<bool> ChangePasswordAsync(Guid id, string currentPassword, string newPassword, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
                throw new BusinessException("Mevcut şifre yanlış.");

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await unitOfWork.ApplicationUserRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);

            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<bool> LockUserAsync(Guid id, DateTime lockoutEnd, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            user.LockoutEnd = lockoutEnd;
            await unitOfWork.ApplicationUserRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<bool> UnlockUserAsync(Guid id, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            user.LockoutEnd = null;
            await unitOfWork.ApplicationUserRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException("İşlem sırasında hata oluştu", ex);
        }
    }

    public async Task<bool> ChangeUserStatusAsync(Guid id, UserStatus status, CancellationToken cancellationToken = default)
    {
        try
        {
            var user = await unitOfWork.ApplicationUserRepository.GetByIdAsync(id, cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            user.Status = status;
            await unitOfWork.ApplicationUserRepository.UpdateAsync(user, cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"İşlem sırasında hata oluştu. id: {id}, status: {status}", ex);
        }
    }
}