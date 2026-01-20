using DairyManagement.Application.DTOs;

namespace DairyManagement.Application.Interfaces;

/// <summary>
/// Service interface for stock management operations
/// </summary>
public interface IStockService
{
    /// <summary>
    /// Records a product purchase (increases stock)
    /// </summary>
    Task RecordPurchaseAsync(int productId, decimal quantity, decimal unitPrice, string? supplierName, string? invoiceNumber, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Records a product sale (decreases stock)
    /// </summary>
    Task RecordSaleAsync(int customerId, int productId, int cycleId, decimal quantity, decimal unitPrice, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets products that need reordering
    /// </summary>
    Task<IEnumerable<ProductDto>> GetProductsNeedingReorderAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets current stock level for a product
    /// </summary>
    Task<decimal> GetCurrentStockAsync(int productId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if sufficient stock is available for a sale
    /// </summary>
    Task<bool> CheckStockAvailabilityAsync(int productId, decimal requiredQuantity, CancellationToken cancellationToken = default);
}
