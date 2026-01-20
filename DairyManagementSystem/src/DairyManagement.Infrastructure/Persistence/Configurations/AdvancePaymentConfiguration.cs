using DairyManagement.Domain.Entities;
using DairyManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for AdvancePayment
/// </summary>
public class AdvancePaymentConfiguration : IEntityTypeConfiguration<AdvancePayment>
{
    public void Configure(EntityTypeBuilder<AdvancePayment> builder)
    {
        // Table name
        builder.ToTable("advance_payments");

        // Primary key
        builder.HasKey(ap => ap.AdvanceId);
        builder.Property(ap => ap.AdvanceId)
            .HasColumnName("advance_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(ap => ap.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(ap => ap.CycleId)
            .HasColumnName("cycle_id")
            .IsRequired();

        builder.Property(ap => ap.PaymentDate)
            .HasColumnName("payment_date")
            .HasColumnType("timestamp")
            .IsRequired();

        builder.Property<decimal>("_amount")
            .HasColumnName("amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(ap => ap.PaymentMode)
            .HasColumnName("payment_mode")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<PaymentMode>(v, true))
            .HasDefaultValue(PaymentMode.Cash);

        builder.Property(ap => ap.ReferenceNumber)
            .HasColumnName("reference_number")
            .HasMaxLength(50);

        builder.Property(ap => ap.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        // Audit fields
        builder.Property(ap => ap.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(ap => ap.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(ap => ap.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(ap => ap.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(ap => ap.CustomerId)
            .HasDatabaseName("idx_advance_payments_customer");

        builder.HasIndex(ap => ap.CycleId)
            .HasDatabaseName("idx_advance_payments_cycle");

        builder.HasIndex(ap => ap.PaymentDate)
            .HasDatabaseName("idx_advance_payments_date");

        // Relationships
        builder.HasOne(ap => ap.Customer)
            .WithMany()
            .HasForeignKey(ap => ap.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(ap => ap.MilkCycle)
            .WithMany(mc => mc.AdvancePayments)
            .HasForeignKey(ap => ap.CycleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore value object properties
        builder.Ignore(ap => ap.Amount);
    }
}
