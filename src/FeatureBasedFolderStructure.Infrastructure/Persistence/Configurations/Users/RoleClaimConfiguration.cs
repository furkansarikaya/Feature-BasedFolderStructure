using FeatureBasedFolderStructure.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Configurations.Users;

public class RoleClaimConfiguration : IEntityTypeConfiguration<RoleClaim>
{
    public void Configure(EntityTypeBuilder<RoleClaim> builder)
    {
        builder.Property(rc => rc.ClaimType)
            .HasMaxLength(256)
            .IsRequired();

        builder.Property(rc => rc.ClaimValue)
            .HasMaxLength(256)
            .IsRequired();

        builder.HasOne(rc => rc.Role)
            .WithMany(r => r.RoleClaims)
            .HasForeignKey(rc => rc.RoleId)
            .IsRequired();

        builder.HasIndex(rc => new { rc.RoleId, rc.ClaimType })
            .IsUnique();
    }
}