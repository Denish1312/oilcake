using DairyManagement.Domain.Entities;
using DairyManagement.Domain.Enums;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Product entity
/// </summary>
public interface IProductRepository : IRepository<Product, int>
{
    /// <summary>
    /// Gets a product by product code
    /// </summary>
    Task<Product?> GetByCodeAsync(string productCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active products
    /// </summary>
    Task<IEnumerable<Product>> GetActiveProductsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products that need reordering
    /// </summary>
    Task<IEnumerable<Product>> GetProductsNeedingReorderAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets products by stock status
    /// </summary>
    Task<IEnumerable<Product>> GetProductsByStockStatusAsync(StockStatus status, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a product code already exists
    /// </summary>
    Task<bool> CodeExistsAsync(string productCode, CancellationToken cancellationToken = default);
}
