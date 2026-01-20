using DairyManagement.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore.Storage;
using DairyManagement.Domain.Entities;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Unit of Work implementation
/// Coordinates repository operations and transactions
/// </summary>
public class UnitOfWork : IUnitOfWork
{
    private readonly DairyDbContext _context;
    private IDbContextTransaction? _transaction;

    // Lazy-loaded repositories
    private ICustomerRepository? _customers;
    private IProductRepository? _products;
    private IMilkCycleRepository? _milkCycles;
    private ISettlementRepository? _settlements;
    private IRepository<ProductPurchase, int>? _productPurchases;
    private IRepository<ProductSale, int>? _productSales;
    private IRepository<AdvancePayment, int>? _advancePayments;

    public UnitOfWork(DairyDbContext context)
    {
        _context = context;
    }

    public ICustomerRepository Customers =>
        _customers ??= new CustomerRepository(_context);

    public IProductRepository Products =>
        _products ??= new ProductRepository(_context);

    public IMilkCycleRepository MilkCycles =>
        _milkCycles ??= new MilkCycleRepository(_context);

    public ISettlementRepository Settlements =>
        _settlements ??= new SettlementRepository(_context);

    public IRepository<ProductPurchase, int> ProductPurchases =>
        _productPurchases ??= new Repository<ProductPurchase, int>(_context);

    public IRepository<ProductSale, int> ProductSales =>
        _productSales ??= new Repository<ProductSale, int>(_context);

    public IRepository<AdvancePayment, int> AdvancePayments =>
        _advancePayments ??= new Repository<AdvancePayment, int>(_context);

    private IUserRepository? _users;
    public IUserRepository Users => _users ??= new UserRepository(_context);

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
    {
        _transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            throw new InvalidOperationException("No active transaction to commit");

        try
        {
            await _context.SaveChangesAsync(cancellationToken);
            await _transaction.CommitAsync(cancellationToken);
        }
        catch
        {
            await RollbackTransactionAsync(cancellationToken);
            throw;
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
    {
        if (_transaction == null)
            return;

        try
        {
            await _transaction.RollbackAsync(cancellationToken);
        }
        finally
        {
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}
