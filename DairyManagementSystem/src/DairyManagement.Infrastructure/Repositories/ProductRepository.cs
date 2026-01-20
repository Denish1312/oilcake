using DairyManagement.Domain.Entities;
using DairyManagement.Domain.Enums;
using DairyManagement.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Product repository implementation
/// </summary>
public class ProductRepository : Repository<Product, int>, IProductRepository
{
    public ProductRepository(DairyDbContext context) : base(context)
    {
    }

    public async Task<Product?> GetByCodeAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(p => p.ProductCode == productCode.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(p => p.IsActive)
            .OrderBy(p => p.ProductCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsNeedingReorderAsync(CancellationToken cancellationToken = default)
    {
        // Products where current_stock <= reorder_level
        return await DbSet
            .Where(p => p.IsActive && EF.Property<decimal>(p, "_currentStock") <= EF.Property<decimal>(p, "_reorderLevel"))
            .OrderBy(p => p.ProductName)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsByStockStatusAsync(StockStatus status, CancellationToken cancellationToken = default)
    {
        // This is a simplified implementation
        // In a real application, you might want to use a database function or computed column
        var products = await GetActiveProductsAsync(cancellationToken);
        return products.Where(p => p.GetStockStatus() == status).ToList();
    }

    public async Task<bool> CodeExistsAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(p => p.ProductCode == productCode.ToUpperInvariant(), cancellationToken);
    }
}
