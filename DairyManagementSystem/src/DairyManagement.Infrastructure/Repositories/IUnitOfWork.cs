using DairyManagement.Domain.Entities;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Unit of Work pattern interface
/// Coordinates multiple repository operations in a single transaction
/// </summary>
public interface IUnitOfWork : IDisposable
{
    // Repository properties
    ICustomerRepository Customers { get; }
    IProductRepository Products { get; }
    IMilkCycleRepository MilkCycles { get; }
    ISettlementRepository Settlements { get; }
    
    // Additional repositories for transaction history
    IRepository<ProductPurchase, int> ProductPurchases { get; }
    IRepository<ProductSale, int> ProductSales { get; }
    IRepository<AdvancePayment, int> AdvancePayments { get; }
    IUserRepository Users { get; }

    /// <summary>
    /// Saves all changes made in this unit of work
    /// </summary>
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Begins a database transaction
    /// </summary>
    Task BeginTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Commits the current transaction
    /// </summary>
    Task CommitTransactionAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Rolls back the current transaction
    /// </summary>
    Task RollbackTransactionAsync(CancellationToken cancellationToken = default);
}
