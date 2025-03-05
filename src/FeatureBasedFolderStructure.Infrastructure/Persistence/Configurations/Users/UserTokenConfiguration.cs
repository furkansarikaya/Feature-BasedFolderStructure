using FeatureBasedFolderStructure.Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Configurations.Users;

public class UserTokenConfiguration : IEntityTypeConfiguration<UserToken>
{
    public void Configure(EntityTypeBuilder<UserToken> builder)
    {
        builder.Property(ut => ut.TokenType)
            .IsRequired();

        builder.Property(ut => ut.TokenValue)
            .IsRequired();

        builder.Property(ut => ut.ExpiryDate);

        builder.HasOne(ut => ut.User)
            .WithMany(u => u.UserTokens)
            .HasForeignKey(ut => ut.UserId)
            .IsRequired();

        builder.HasIndex(ut => new { ut.UserId, ut.TokenType,ut.TokenValue, ut.IsDeleted })
            .IsUnique();
    }
}

