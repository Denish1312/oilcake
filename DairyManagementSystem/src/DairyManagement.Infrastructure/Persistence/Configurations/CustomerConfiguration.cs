using DairyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace DairyManagement.Infrastructure.Persistence.Configurations;

/// <summary>
/// Entity configuration for Customer
/// </summary>
public class CustomerConfiguration : IEntityTypeConfiguration<Customer>
{
    public void Configure(EntityTypeBuilder<Customer> builder)
    {
        // Table name
        builder.ToTable("customers");

        // Primary key
        builder.HasKey(c => c.CustomerId);
        builder.Property(c => c.CustomerId)
            .HasColumnName("customer_id")
            .ValueGeneratedOnAdd();

        // Properties
        builder.Property(c => c.CustomerCode)
            .HasColumnName("customer_code")
            .HasMaxLength(20)
            .IsRequired();

        builder.Property(c => c.FullName)
            .HasColumnName("full_name")
            .HasMaxLength(200)
            .IsRequired();

        builder.Property(c => c.PhoneNumber)
            .HasColumnName("phone_number")
            .HasMaxLength(15);

        builder.Property(c => c.Address)
            .HasColumnName("address")
            .HasColumnType("text");

        builder.Property(c => c.Village)
            .HasColumnName("village")
            .HasMaxLength(100);

        builder.Property(c => c.IsActive)
            .HasColumnName("is_active")
            .IsRequired()
            .HasDefaultValue(true);

        // Audit fields
        builder.Property(c => c.CreatedAt)
            .HasColumnName("created_at")
            .IsRequired();

        builder.Property(c => c.UpdatedAt)
            .HasColumnName("updated_at")
            .IsRequired();

        builder.Property(c => c.CreatedBy)
            .HasColumnName("created_by")
            .HasMaxLength(100);

        builder.Property(c => c.UpdatedBy)
            .HasColumnName("updated_by")
            .HasMaxLength(100);

        // Indexes
        builder.HasIndex(c => c.CustomerCode)
            .IsUnique()
            .HasDatabaseName("idx_customers_code");

        builder.HasIndex(c => c.IsActive)
            .HasDatabaseName("idx_customers_active");

        builder.HasIndex(c => c.FullName)
            .HasDatabaseName("idx_customers_name");

        // Relationships
        builder.HasMany(c => c.MilkCycles)
            .WithOne(mc => mc.Customer)
            .HasForeignKey(mc => mc.CustomerId)
            .OnDelete(DeleteBehavior.Restrict);
    }
}
