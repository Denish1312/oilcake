using DairyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for ProductPurchase
/// </summary>
public class ProductPurchaseConfiguration : IEntityTypeConfiguration<ProductPurchase>
{
    public void Configure(EntityTypeBuilder<ProductPurchase> builder)
    {
        // Table name
        builder.ToTable("product_purchases");

        // Primary key
        builder.HasKey(pp => pp.PurchaseId);
        builder.Property(pp => pp.PurchaseId)
            .HasColumnName("purchase_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(pp => pp.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(pp => pp.PurchaseDate)
            .HasColumnName("purchase_date")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property<decimal>("_quantity")
            .HasColumnName("quantity")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property<string>("_unit")
            .HasColumnName("unit_of_measure")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property<decimal>("_unitPrice")
            .HasColumnName("unit_price")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property<decimal>("_totalAmount")
            .HasColumnName("total_amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(pp => pp.SupplierName)
            .HasColumnName("supplier_name")
            .HasMaxLength(200);

        builder.Property(pp => pp.InvoiceNumber)
            .HasColumnName("invoice_number")
            .HasMaxLength(50);

        builder.Property(pp => pp.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        // Audit fields
        builder.Property(pp => pp.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(pp => pp.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(pp => pp.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(pp => pp.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(pp => pp.ProductId)
            .HasDatabaseName("idx_product_purchases_product");

        builder.HasIndex(pp => pp.PurchaseDate)
            .HasDatabaseName("idx_product_purchases_date");

        builder.HasIndex(pp => pp.InvoiceNumber)
            .HasDatabaseName("idx_product_purchases_invoice");

        // Relationships
        builder.HasOne(pp => pp.Product)
            .WithMany()
            .HasForeignKey(pp => pp.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore value object properties
        builder.Ignore(pp => pp.Quantity);
        builder.Ignore(pp => pp.UnitPrice);
        builder.Ignore(pp => pp.TotalAmount);
    }
}
