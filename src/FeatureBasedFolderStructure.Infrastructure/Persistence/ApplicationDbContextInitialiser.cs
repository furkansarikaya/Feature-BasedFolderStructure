using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.Enums;
using FeatureBasedFolderStructure.Infrastructure.Persistence.Context;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence;

public class ApplicationDbContextInitialiser(
    ILogger<ApplicationDbContextInitialiser> logger,
    ApplicationDbContext context)
{
    public async Task InitialiseAsync()
    {
        try
        {
            if (context.Database.IsNpgsql())
            {
                logger.LogInformation("Initialising PostgreSQL database start");
                await context.Database.MigrateAsync();
                logger.LogInformation("Initialising PostgreSQL database end");
            }
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while initialising the database."!);
            throw;
        }
    }

    public async Task SeedAsync()
    {
        try
        {
            await TrySeedAsync();
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "An error occurred while seeding the database."!);
            throw;
        }
    }

    private async Task TrySeedAsync()
    {
        logger.LogInformation("Seeding database start");

        // Default data seeding
        if (!context.Categories.Any())
        {
            context.Categories.Add(new Category
            {
                Name = "Default Category",
                Description = "Default Category Description"
            });
        }

        var role = context.Roles.FirstOrDefault();
        if (role == null)
        {
            role = new Role
            {
                Name = "Admin",
                NormalizedName = "ADMIN"
            };
            context.Roles.Add(role);
            await context.SaveChangesAsync();
        }

        if (!context.RoleClaims.Any())
        {
            context.RoleClaims.Add(new RoleClaim
            {
                RoleId = role.Id,
                ClaimType = ClaimType.Permission,
                ClaimValue = "Admin"
            });
        }

        var user = context.ApplicationUsers.FirstOrDefault();
        if (user == null)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            user = new ApplicationUser
            {
                UserName = "admin",
                Email = "admin@admin.com",
                PasswordHash = passwordHasher.HashPassword(null, "pwd"),
            };
            context.ApplicationUsers.Add(user);
            await context.SaveChangesAsync();
        }
        
        if (!context.UserRoles.Any())
        {
            context.UserRoles.Add(new UserRole
            {
                UserId = user.Id,
                RoleId = role.Id
            });
        }

        await context.SaveChangesAsync();
        logger.LogInformation("Seeding database end");
    }
}