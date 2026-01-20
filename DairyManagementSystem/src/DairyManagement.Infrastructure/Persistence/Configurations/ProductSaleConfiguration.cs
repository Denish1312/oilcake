using DairyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for ProductSale
/// </summary>
public class ProductSaleConfiguration : IEntityTypeConfiguration<ProductSale>
{
    public void Configure(EntityTypeBuilder<ProductSale> builder)
    {
        // Table name
        builder.ToTable("product_sales");

        // Primary key
        builder.HasKey(ps => ps.SaleId);
        builder.Property(ps => ps.SaleId)
            .HasColumnName("sale_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(ps => ps.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(ps => ps.ProductId)
            .HasColumnName("product_id")
            .IsRequired();

        builder.Property(ps => ps.CycleId)
            .HasColumnName("cycle_id")
            .IsRequired();

        builder.Property(ps => ps.SaleDate)
            .HasColumnName("sale_date")
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

        builder.Property(ps => ps.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        // Audit fields
        builder.Property(ps => ps.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ps => ps.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(ps => ps.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(ps => ps.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(ps => ps.CustomerId)
            .HasDatabaseName("idx_product_sales_customer");

        builder.HasIndex(ps => ps.ProductId)
            .HasDatabaseName("idx_product_sales_product");

        builder.HasIndex(ps => ps.CycleId)
            .HasDatabaseName("idx_product_sales_cycle");

        builder.HasIndex(ps => ps.SaleDate)
            .HasDatabaseName("idx_product_sales_date");

        // Relationships
        builder.HasOne(ps => ps.Customer)
            .WithMany()
            .HasForeignKey(ps => ps.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.Product)
            .WithMany()
            .HasForeignKey(ps => ps.ProductId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ps => ps.MilkCycle)
            .WithMany(mc => mc.ProductSales)
            .HasForeignKey(ps => ps.CycleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore value object properties
        builder.Ignore(ps => ps.Quantity);
        builder.Ignore(ps => ps.UnitPrice);
        builder.Ignore(ps => ps.TotalAmount);
    }
}
