using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Repositories;

namespace DairyManagement.Application.Services;

/// <summary>
/// Stock service implementation
/// Handles inventory management operations
/// </summary>
public class StockService : IStockService
{
    private readonly IUnitOfWork _unitOfWork;

    public StockService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task RecordPurchaseAsync(
        int productId, 
        decimal quantity, 
        decimal unitPrice, 
        string? supplierName, 
        string? invoiceNumber, 
        string username, 
        CancellationToken cancellationToken = default)
    {
        // Get product
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException($"Product {productId} not found");

        // Create purchase record
        var purchase = ProductPurchase.Create(
            productId,
            quantity,
            product.UnitOfMeasure,
            unitPrice,
            supplierName,
            invoiceNumber
        );
        purchase.SetCreatedBy(username);

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Save purchase record
            await _unitOfWork.ProductPurchases.AddAsync(purchase, cancellationToken);
            
            // Increase stock
            product.IncreaseStock(quantity);
            product.SetUpdatedBy(username);
            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task RecordSaleAsync(
        int customerId, 
        int productId, 
        int cycleId, 
        decimal quantity, 
        decimal unitPrice, 
        string username, 
        CancellationToken cancellationToken = default)
    {
        // Get product
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException($"Product {productId} not found");

        // Get cycle
        var cycle = await _unitOfWork.MilkCycles.GetByIdAsync(cycleId, cancellationToken);
        if (cycle == null)
            throw new InvalidOperationException($"Milk cycle {cycleId} not found");

        // Validate cycle is not settled
        if (cycle.IsSettled)
            throw new InvalidOperationException("Cannot add sales to a settled cycle");

        // Validate customer matches cycle
        if (cycle.CustomerId != customerId)
            throw new InvalidOperationException("Customer does not match the cycle");

        // Check stock availability
        if (!product.HasSufficientStock(quantity))
            throw new InvalidOperationException($"Insufficient stock. Available: {product.CurrentStock.Value}, Required: {quantity}");

        // Create sale record
        var sale = ProductSale.Create(
            customerId,
            productId,
            cycleId,
            quantity,
            product.UnitOfMeasure,
            unitPrice
        );
        sale.SetCreatedBy(username);

        // Begin transaction
        await _unitOfWork.BeginTransactionAsync(cancellationToken);
        try
        {
            // Save sale record
            await _unitOfWork.ProductSales.AddAsync(sale, cancellationToken);
            
            // Decrease stock
            product.DecreaseStock(quantity);
            product.SetUpdatedBy(username);
            await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
            
            await _unitOfWork.SaveChangesAsync(cancellationToken);
            await _unitOfWork.CommitTransactionAsync(cancellationToken);
        }
        catch
        {
            await _unitOfWork.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }

    public async Task<IEnumerable<ProductDto>> GetProductsNeedingReorderAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetProductsNeedingReorderAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<decimal> GetCurrentStockAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException($"Product {productId} not found");

        return product.CurrentStock.Value;
    }

    public async Task<bool> CheckStockAvailabilityAsync(int productId, decimal requiredQuantity, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            return false;

        return product.HasSufficientStock(requiredQuantity);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Description = product.Description,
            UnitOfMeasure = product.UnitOfMeasure,
            UnitPrice = product.UnitPrice.Amount,
            CurrentStock = product.CurrentStock.Value,
            ReorderLevel = product.ReorderLevel.Value,
            StockStatus = product.GetStockStatus().ToString(),
            IsActive = product.IsActive
        };
    }
}
