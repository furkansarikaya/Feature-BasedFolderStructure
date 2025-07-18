using FeatureBasedFolderStructure.Application.Features.v1.Auth.Interfaces.Users;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FS.AspNetCore.ResponseWrapper.Exceptions;
using FS.AutoServiceDiscovery.Extensions.Attributes;
using FS.EntityFramework.Library.UnitOfWorks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace FeatureBasedFolderStructure.Infrastructure.Features.Auth.Services;

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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.GetByIdAsync(id, true, cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();

            var user = await applicationUserRepository.GetQueryable()
                .Include(u => u.UserRoles)
                .ThenInclude(ur => ur.Role)
                .ThenInclude(r => r.RoleClaims)
                .FirstOrDefaultAsync(u => u.Id == id, cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.FirstOrDefaultAsync(predicate: e => e.Email == email, cancellationToken: cancellationToken);
            return user ?? throw new NotFoundException(nameof(ApplicationUser), email);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var emailExists = await applicationUserRepository.ExistsAsync(e => e.Email == user.Email, cancellationToken);
            if (emailExists)
                throw new BusinessException("Bu e-posta adresi zaten kullanımda.");

            user.PasswordHash = _passwordHasher.HashPassword(user, password);

            var result = await applicationUserRepository.AddAsync(user, cancellationToken: cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var existingUser = await applicationUserRepository.GetByIdAsync(user.Id, true, cancellationToken);
            if (existingUser == null)
                throw new NotFoundException(nameof(ApplicationUser), user.Id);

            user.PasswordHash = existingUser.PasswordHash;
            await applicationUserRepository.UpdateAsync(user, cancellationToken: cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            await applicationUserRepository.DeleteAsync(user, cancellationToken: cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);
            
            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await applicationUserRepository.UpdateAsync(user, cancellationToken: cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            var verificationResult = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, currentPassword);
            if (verificationResult == PasswordVerificationResult.Failed)
                throw new BusinessException("Mevcut şifre yanlış.");

            user.PasswordHash = _passwordHasher.HashPassword(user, newPassword);
            await applicationUserRepository.UpdateAsync(user, cancellationToken: cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            user.LockoutEnd = lockoutEnd;
            await applicationUserRepository.UpdateAsync(user, cancellationToken: cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            user.LockoutEnd = null;
            await applicationUserRepository.UpdateAsync(user, cancellationToken: cancellationToken);
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
            var applicationUserRepository = unitOfWork.GetRepository<ApplicationUser, Guid>();
            var user = await applicationUserRepository.GetByIdAsync(id, cancellationToken: cancellationToken);
            if (user == null)
                throw new NotFoundException(nameof(ApplicationUser), id);

            user.Status = status;
            await applicationUserRepository.UpdateAsync(user, cancellationToken: cancellationToken);
            await unitOfWork.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            throw new ApplicationException($"İşlem sırasında hata oluştu. id: {id}, status: {status}", ex);
        }
    }
}