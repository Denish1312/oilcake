using DairyManagement.Domain.Enums;
using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents a feed product (oil cake, cotton seed, etc.)
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int ProductId { get; private set; }

    /// <summary>
    /// Unique product code (e.g., PROD001)
    /// </summary>
    public string ProductCode { get; private set; } = string.Empty;

    /// <summary>
    /// Product name
    /// </summary>
    public string ProductName { get; private set; } = string.Empty;

    /// <summary>
    /// Product description (optional)
    /// </summary>
    public string? Description { get; private set; }

    /// <summary>
    /// Unit of measure (KG, BAG, etc.)
    /// </summary>
    public string UnitOfMeasure { get; private set; } = string.Empty;

    /// <summary>
    /// Current unit price
    /// </summary>
    private decimal _unitPrice;
    public Money UnitPrice => Money.FromAmount(_unitPrice);

    /// <summary>
    /// Current stock quantity
    /// </summary>
    private decimal _currentStock;
    public Quantity CurrentStock => Quantity.Create(_currentStock, UnitOfMeasure);

    /// <summary>
    /// Reorder level (minimum stock threshold)
    /// </summary>
    private decimal _reorderLevel;
    public Quantity ReorderLevel => Quantity.Create(_reorderLevel, UnitOfMeasure);

    /// <summary>
    /// Whether the product is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Product() { }

    /// <summary>
    /// Creates a new product
    /// </summary>
    public static Product Create(
        string productCode,
        string productName,
        string unitOfMeasure,
        decimal unitPrice,
        decimal reorderLevel,
        string? description = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(productCode))
            throw new ArgumentException("Product code is required", nameof(productCode));

        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required", nameof(productName));

        if (string.IsNullOrWhiteSpace(unitOfMeasure))
            throw new ArgumentException("Unit of measure is required", nameof(unitOfMeasure));

        if (productCode.Length > 20)
            throw new ArgumentException("Product code cannot exceed 20 characters", nameof(productCode));

        if (productName.Length > 200)
            throw new ArgumentException("Product name cannot exceed 200 characters", nameof(productName));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(reorderLevel));

        return new Product
        {
            ProductCode = productCode.ToUpperInvariant(),
            ProductName = productName,
            Description = description,
            UnitOfMeasure = unitOfMeasure.ToUpperInvariant(),
            _unitPrice = unitPrice,
            _currentStock = 0, // Starts with zero stock
            _reorderLevel = reorderLevel,
            IsActive = true
        };
    }

    /// <summary>
    /// Updates product information
    /// </summary>
    public void Update(
        string productName,
        decimal unitPrice,
        decimal reorderLevel,
        string? description = null)
    {
        if (string.IsNullOrWhiteSpace(productName))
            throw new ArgumentException("Product name is required", nameof(productName));

        if (productName.Length > 200)
            throw new ArgumentException("Product name cannot exceed 200 characters", nameof(productName));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (reorderLevel < 0)
            throw new ArgumentException("Reorder level cannot be negative", nameof(reorderLevel));

        ProductName = productName;
        _unitPrice = unitPrice;
        _reorderLevel = reorderLevel;
        Description = description;
    }

    /// <summary>
    /// Increases stock (from purchase)
    /// </summary>
    public void IncreaseStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        _currentStock += quantity;
    }

    /// <summary>
    /// Decreases stock (from sale)
    /// </summary>
    public void DecreaseStock(decimal quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (_currentStock < quantity)
            throw new InvalidOperationException($"Insufficient stock. Available: {_currentStock}, Required: {quantity}");

        _currentStock -= quantity;
    }

    /// <summary>
    /// Checks if there is sufficient stock for a sale
    /// </summary>
    public bool HasSufficientStock(decimal requiredQuantity)
    {
        return _currentStock >= requiredQuantity;
    }

    /// <summary>
    /// Gets the current stock status
    /// </summary>
    public StockStatus GetStockStatus()
    {
        if (_currentStock <= 0)
            return StockStatus.OutOfStock;

        if (_currentStock <= _reorderLevel)
            return StockStatus.LowStock;

        if (_currentStock <= _reorderLevel * 1.5m)
            return StockStatus.MediumStock;

        return StockStatus.GoodStock;
    }

    /// <summary>
    /// Checks if stock needs reordering
    /// </summary>
    public bool NeedsReorder()
    {
        return _currentStock <= _reorderLevel;
    }

    /// <summary>
    /// Deactivates the product
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Reactivates the product
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    public override string ToString()
    {
        return $"{ProductCode} - {ProductName} ({CurrentStock})";
    }
}
