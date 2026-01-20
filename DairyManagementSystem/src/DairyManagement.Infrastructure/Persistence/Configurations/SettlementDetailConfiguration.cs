using DairyManagement.Domain.Entities;
using DairyManagement.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for SettlementDetail
/// </summary>
public class SettlementDetailConfiguration : IEntityTypeConfiguration<SettlementDetail>
{
    public void Configure(EntityTypeBuilder<SettlementDetail> builder)
    {
        // Table name
        builder.ToTable("settlement_details");

        // Primary key
        builder.HasKey(sd => sd.DetailId);
        builder.Property(sd => sd.DetailId)
            .HasColumnName("detail_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(sd => sd.SettlementId)
            .HasColumnName("settlement_id")
            .IsRequired();

        builder.Property(sd => sd.DetailType)
            .HasColumnName("detail_type")
            .HasMaxLength(20)
            .IsRequired()
            .HasConversion(
                v => v.ToString().ToUpperInvariant(),
                v => Enum.Parse<SettlementDetailType>(v, true));

        builder.Property(sd => sd.ReferenceId)
            .HasColumnName("reference_id");

        builder.Property(sd => sd.Description)
            .HasColumnName("description")
            .HasMaxLength(500)
            .IsRequired();

        builder.Property<decimal>("_amount")
            .HasColumnName("amount")
            .HasColumnType("decimal(10,2)")
            .IsRequired();

        builder.Property(sd => sd.TransactionDate)
            .HasColumnName("transaction_date")
            .IsRequired();

        // Audit fields (only CreatedAt for details)
        builder.Property(sd => sd.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        // Indexes
        builder.HasIndex(sd => sd.SettlementId)
            .HasDatabaseName("idx_settlement_details_settlement");

        builder.HasIndex(sd => sd.DetailType)
            .HasDatabaseName("idx_settlement_details_type");

        // Relationships
        builder.HasOne(sd => sd.Settlement)
            .WithMany(s => s.Details)
            .HasForeignKey(sd => sd.SettlementId)
            .OnDelete(DeleteBehavior.Cascade);

        // Ignore value object properties
        builder.Ignore(sd => sd.Amount);
    }
}
