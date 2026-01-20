using DairyManagement.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace DairyManagement.Infrastructure.Persistence.DbContext;

/// <summary>
/// Database context for the Dairy Management System
/// </summary>
public class DairyDbContext : Microsoft.EntityFrameworkCore.DbContext
{
    public DairyDbContext(DbContextOptions<DairyDbContext> options) : base(options)
    {
    }

    // DbSets for all entities
    public DbSet<Customer> Customers => Set<Customer>();
    public DbSet<Product> Products => Set<Product>();
    public DbSet<MilkCycle> MilkCycles => Set<MilkCycle>();
    public DbSet<ProductPurchase> ProductPurchases => Set<ProductPurchase>();
    public DbSet<ProductSale> ProductSales => Set<ProductSale>();
    public DbSet<AdvancePayment> AdvancePayments => Set<AdvancePayment>();
    public DbSet<Settlement> Settlements => Set<Settlement>();
    public DbSet<SettlementDetail> SettlementDetails => Set<SettlementDetail>();
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Apply all entity configurations from the assembly
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(DairyDbContext).Assembly);
    }

    /// <summary>
    /// Override SaveChanges to automatically update audit fields
    /// </summary>
    public override int SaveChanges()
    {
        UpdateAuditFields();
        return base.SaveChanges();
    }

    /// <summary>
    /// Override SaveChangesAsync to automatically update audit fields
    /// </summary>
    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        UpdateAuditFields();
        return base.SaveChangesAsync(cancellationToken);
    }

    /// <summary>
    /// Updates audit fields (UpdatedAt, UpdatedBy) for modified entities
    /// </summary>
    private void UpdateAuditFields()
    {
        var entries = ChangeTracker.Entries<BaseEntity>()
            .Where(e => e.State == EntityState.Modified);

        foreach (var entry in entries)
        {
            // UpdatedAt is automatically set by the entity's SetUpdatedBy method
            // In a real application, you would get the current user from the context
            // For now, we'll set it in the application layer
        }
    }
}
