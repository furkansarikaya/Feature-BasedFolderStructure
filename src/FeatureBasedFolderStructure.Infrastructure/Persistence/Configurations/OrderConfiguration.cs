using FeatureBasedFolderStructure.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Configurations;

public class OrderConfiguration : IEntityTypeConfiguration<Order>
{
    public void Configure(EntityTypeBuilder<Order> builder)
    {
        builder.Property(o => o.OrderNumber)
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(o => o.TotalAmount)
            .HasPrecision(18, 2)
            .IsRequired();
        
        builder.OwnsOne(o => o.ShippingAddress, sa =>
        {
            sa.Property(a => a.Street).HasMaxLength(200).IsRequired();
            sa.Property(a => a.City).HasMaxLength(100).IsRequired();
            sa.Property(a => a.State).HasMaxLength(100).IsRequired();
            sa.Property(a => a.Country).HasMaxLength(100).IsRequired();
            sa.Property(a => a.ZipCode).HasMaxLength(20).IsRequired();
        });

        builder.HasMany(o => o.OrderItems)
            .WithOne()
            .HasForeignKey(oi => oi.OrderId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}