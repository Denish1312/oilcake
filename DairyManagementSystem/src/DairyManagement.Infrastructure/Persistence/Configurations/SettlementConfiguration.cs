using DairyManagement.Domain.Entities;
using DairyManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for Settlement
/// </summary>
public class SettlementConfiguration : IEntityTypeConfiguration<Settlement>
{
    public void Configure(EntityTypeBuilder<Settlement> builder)
    {
        // Table name
        builder.ToTable("settlements");

        // Primary key
        builder.HasKey(s => s.SettlementId);
        builder.Property(s => s.SettlementId)
            .HasColumnName("settlement_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(s => s.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(s => s.CycleId)
            .HasColumnName("cycle_id")
            .IsRequired();

        builder.Property(s => s.SettlementDate)
            .HasColumnName("settlement_date")
            .HasColumnType("timestamp")
            .IsRequired();

        // Settlement calculation fields
        builder.Property<decimal>("_milkAmount")
            .HasColumnName("milk_amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property<decimal>("_totalProductSales")
            .HasColumnName("total_product_sales")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property<decimal>("_totalAdvancePaid")
            .HasColumnName("total_advance_paid")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property<decimal>("_finalPayable")
            .HasColumnName("final_payable")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        // Payment details
        builder.Property(s => s.PaymentMode)
            .HasColumnName("payment_mode")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<PaymentMode>(v, true))
            .HasDefaultValue(PaymentMode.Cash);

        builder.Property(s => s.PaymentReference)
            .HasColumnName("payment_reference")
            .HasMaxLength(50);

        builder.Property(s => s.PaymentDate)
            .HasColumnName("payment_date")
            .HasColumnType("timestamp");

        builder.Property(s => s.IsPaid)
            .HasColumnName("is_paid")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(s => s.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        // Audit fields
        builder.Property(s => s.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(s => s.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(s => s.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(s => s.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(s => s.CustomerId)
            .HasDatabaseName("idx_settlements_customer");

        builder.HasIndex(s => s.CycleId)
            .IsUnique()
            .HasDatabaseName("idx_settlements_cycle");

        builder.HasIndex(s => s.SettlementDate)
            .HasDatabaseName("idx_settlements_date");

        builder.HasIndex(s => s.IsPaid)
            .HasDatabaseName("idx_settlements_paid");

        // Relationships
        builder.HasOne(s => s.Customer)
            .WithMany()
            .HasForeignKey(s => s.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(s => s.MilkCycle)
            .WithOne(mc => mc.Settlement)
            .HasForeignKey<Settlement>(s => s.CycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(s => s.Details)
            .WithOne(sd => sd.Settlement)
            .HasForeignKey(sd => sd.SettlementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore value object properties
        builder.Ignore(s => s.MilkAmount);
        builder.Ignore(s => s.TotalProductSales);
        builder.Ignore(s => s.TotalAdvancePaid);
        builder.Ignore(s => s.FinalPayable);
    }
}
