using DairyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for Product
/// </summary>
public class ProductConfiguration : IEntityTypeConfiguration<Product>
{
    public void Configure(EntityTypeBuilder<Product> builder)
    {
        // Table name
        builder.ToTable("products");

        // Primary key
        builder.HasKey(p => p.ProductId);
        builder.Property(p => p.ProductId)
            .HasColumnName("product_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(p => p.ProductCode)
            .HasColumnName("product_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(p => p.ProductName)
            .HasColumnName("product_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(p => p.Description)
            .HasColumnName("description")
            .HasColumnType("text");

        builder.Property(p => p.UnitOfMeasure)
            .HasColumnName("unit_of_measure")
            .HasMaxLength(20)
            .IsRequired()
            .HasDefaultValue("KG");

        // Value object properties - stored as backing fields
        builder.Property<decimal>("_unitPrice")
            .HasColumnName("unit_price")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property<decimal>("_currentStock")
            .HasColumnName("current_stock")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property<decimal>("_reorderLevel")
            .HasColumnName("reorder_level")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(p => p.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        // Audit fields
        builder.Property(p => p.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(p => p.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(p => p.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(p => p.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(p => p.ProductCode)
            .IsUnique()
            .HasDatabaseName("idx_products_code");

        builder.HasIndex(p => p.IsActive)
            .HasDatabaseName("idx_products_active");

        builder.HasIndex("_currentStock")
            .HasDatabaseName("idx_products_stock");

        // Ignore value object properties (they're computed from backing fields)
        builder.Ignore(p => p.UnitPrice);
        builder.Ignore(p => p.CurrentStock);
        builder.Ignore(p => p.ReorderLevel);
    }
}
