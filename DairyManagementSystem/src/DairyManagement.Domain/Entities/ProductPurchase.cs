using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents a product inventory purchase (stock in)
/// </summary>
public class ProductPurchase : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int PurchaseId { get; private set; }

    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; private set; }

    /// <summary>
    /// Purchase date and time
    /// </summary>
    public DateTime PurchaseDate { get; private set; }

    /// <summary>
    /// Quantity purchased
    /// </summary>
    private decimal _quantity;
    private string _unit = string.Empty;
    public Quantity Quantity => Quantity.Create(_quantity, _unit);

    /// <summary>
    /// Unit price at time of purchase
    /// </summary>
    private decimal _unitPrice;
    public Money UnitPrice => Money.FromAmount(_unitPrice);

    /// <summary>
    /// Total amount (quantity × unit price)
    /// </summary>
    private decimal _totalAmount;
    public Money TotalAmount => Money.FromAmount(_totalAmount);

    /// <summary>
    /// Supplier name (optional)
    /// </summary>
    public string? SupplierName { get; private set; }

    /// <summary>
    /// Invoice number (optional)
    /// </summary>
    public string? InvoiceNumber { get; private set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; private set; }

    // Navigation properties
    public Product Product { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private ProductPurchase() { }

    /// <summary>
    /// Creates a new product purchase
    /// </summary>
    public static ProductPurchase Create(
        int productId,
        decimal quantity,
        string unit,
        decimal unitPrice,
        string? supplierName = null,
        string? invoiceNumber = null,
        string? notes = null)
    {
        // Validation
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit is required", nameof(unit));

        var totalAmount = quantity * unitPrice;

        return new ProductPurchase
        {
            ProductId = productId,
            PurchaseDate = DateTime.UtcNow,
            _quantity = quantity,
            _unit = unit.ToUpperInvariant(),
            _unitPrice = unitPrice,
            _totalAmount = totalAmount,
            SupplierName = supplierName,
            InvoiceNumber = invoiceNumber,
            Notes = notes
        };
    }

    /// <summary>
    /// Updates purchase details (before processing)
    /// </summary>
    public void Update(
        decimal quantity,
        decimal unitPrice,
        string? supplierName = null,
        string? invoiceNumber = null,
        string? notes = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        _quantity = quantity;
        _unitPrice = unitPrice;
        _totalAmount = quantity * unitPrice;
        SupplierName = supplierName;
        InvoiceNumber = invoiceNumber;
        Notes = notes;
    }

    public override string ToString()
    {
        return $"Purchase {PurchaseId}: {Quantity} @ {UnitPrice} = {TotalAmount}";
    }
}
