using FeatureBasedFolderStructure.Domain.Entities.Catalogs;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace FeatureBasedFolderStructure.Infrastructure.Persistence.Configurations.Catalogs;

public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        builder.Property(p => p.Name)
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Price)
            .HasPrecision(18, 2)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasMaxLength(1000);
        
        builder.ComplexProperty(p => p.CurrentPrice, cp =>
        {
            cp.Property(m => m.Amount)
                .HasPrecision(18, 2)
                .HasColumnName("current_price_amount")
                .IsRequired();

            cp.Property(m => m.Currency)
                .HasMaxLength(3)
                .HasColumnName("current_price_currency")
                .IsRequired();
        });

        builder.HasOne(p => p.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(p => p.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}