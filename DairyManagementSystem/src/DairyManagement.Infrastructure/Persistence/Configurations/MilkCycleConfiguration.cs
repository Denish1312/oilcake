using DairyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for MilkCycle
/// </summary>
public class MilkCycleConfiguration : IEntityTypeConfiguration<MilkCycle>
{
    public void Configure(EntityTypeBuilder<MilkCycle> builder)
    {
        // Table name
        builder.ToTable("milk_cycles");

        // Primary key
        builder.HasKey(mc => mc.CycleId);
        builder.Property(mc => mc.CycleId)
            .HasColumnName("cycle_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(mc => mc.CustomerId)
            .HasColumnName("customer_id")
            .IsRequired();

        builder.Property(mc => mc.CycleStartDate)
            .HasColumnName("cycle_start_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property(mc => mc.CycleEndDate)
            .HasColumnName("cycle_end_date")
            .HasColumnType("date")
            .IsRequired();

        builder.Property<decimal>("_totalMilkAmount")
            .HasColumnName("total_milk_amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired()
            .HasDefaultValue(0);

        builder.Property(mc => mc.IsSettled)
            .HasColumnName("is_settled")
            .IsRequired()
            .HasDefaultValue(false);

        builder.Property(mc => mc.SettlementDate)
            .HasColumnName("settlement_date")
            .HasColumnType("timestamp");

        builder.Property(mc => mc.Notes)
            .HasColumnName("notes")
            .HasColumnType("text");

        // Audit fields
        builder.Property(mc => mc.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(mc => mc.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(mc => mc.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(mc => mc.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(mc => mc.CustomerId)
            .HasDatabaseName("idx_milk_cycles_customer");

        builder.HasIndex(mc => new { mc.CycleStartDate, mc.CycleEndDate })
            .HasDatabaseName("idx_milk_cycles_dates");

        builder.HasIndex(mc => mc.IsSettled)
            .HasDatabaseName("idx_milk_cycles_settled");

        builder.HasIndex(mc => mc.SettlementDate)
            .HasDatabaseName("idx_milk_cycles_settlement_date");

        // Unique constraint
        builder.HasIndex(mc => new { mc.CustomerId, mc.CycleStartDate })
            .IsUnique()
            .HasDatabaseName("uq_customer_cycle");

        // Relationships
        builder.HasOne(mc => mc.Customer)
            .WithMany(c => c.MilkCycles)
            .HasForeignKey(mc => mc.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(mc => mc.ProductSales)
            .WithOne(ps => ps.MilkCycle)
            .HasForeignKey(ps => ps.CycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasMany(mc => mc.AdvancePayments)
            .WithOne(ap => ap.MilkCycle)
            .HasForeignKey(ap => ap.CycleId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(mc => mc.Settlement)
            .WithOne(s => s.MilkCycle)
            .HasForeignKey<Settlement>(s => s.CycleId)
            .OnDelete(DeleteBehavior.Restrict);

        // Ignore value object properties
        builder.Ignore(mc => mc.TotalMilkAmount);
        builder.Ignore(mc => mc.DateRange);
    }
}
