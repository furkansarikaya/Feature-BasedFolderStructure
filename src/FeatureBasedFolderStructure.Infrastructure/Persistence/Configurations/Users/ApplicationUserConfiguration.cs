using FeatureBasedFolderStructure.Domain.Entities.Users;
using FeatureBasedFolderStructure.Domain.ValueObjects.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Configurations.Users;

public class ApplicationUserConfiguration : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(x => x.FullName)
            .HasConversion(x => x.ToString(), x => FullName.Create(x));

        builder.Property(u => u.Email)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(u => u.PasswordHash)
            .IsRequired();

        builder.Property(u => u.Status)
            .IsRequired();

        builder.Property(u => u.EmailConfirmed)
            .IsRequired();

        builder.Property(u => u.LockoutEnd);

        builder.Property(u => u.AccessFailedCount)
            .IsRequired();

        builder.HasMany(u => u.UserRoles)
            .WithOne()
            .HasForeignKey(ur => ur.UserId)
            .IsRequired();

        builder.HasMany(u => u.UserTokens)
            .WithOne()
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

        builder.HasIndex(u => u.Email)
            .IsUnique();
    }
}