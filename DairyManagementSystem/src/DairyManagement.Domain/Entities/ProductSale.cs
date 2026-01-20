using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents a product sale to a customer (stock out)
/// </summary>
public class ProductSale : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int SaleId { get; private set; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// Product ID
    /// </summary>
    public int ProductId { get; private set; }

    /// <summary>
    /// Milk cycle ID (for settlement calculation)
    /// </summary>
    public int CycleId { get; private set; }

    /// <summary>
    /// Sale date and time
    /// </summary>
    public DateTime SaleDate { get; private set; }

    /// <summary>
    /// Quantity sold
    /// </summary>
    private decimal _quantity;
    private string _unit = string.Empty;
    public Quantity Quantity => Quantity.Create(_quantity, _unit);

    /// <summary>
    /// Unit price at time of sale
    /// </summary>
    private decimal _unitPrice;
    public Money UnitPrice => Money.FromAmount(_unitPrice);

    /// <summary>
    /// Total amount (quantity × unit price)
    /// </summary>
    private decimal _totalAmount;
    public Money TotalAmount => Money.FromAmount(_totalAmount);

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public Product Product { get; private set; } = null!;
    public MilkCycle MilkCycle { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private ProductSale() { }

    /// <summary>
    /// Creates a new product sale
    /// </summary>
    public static ProductSale Create(
        int customerId,
        int productId,
        int cycleId,
        decimal quantity,
        string unit,
        decimal unitPrice,
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

        return new ProductSale
        {
            CustomerId = customerId,
            ProductId = productId,
            CycleId = cycleId,
            SaleDate = DateTime.UtcNow,
            _quantity = quantity,
            _unit = unit.ToUpperInvariant(),
            _unitPrice = unitPrice,
            _totalAmount = totalAmount,
            Notes = notes
        };
    }

    /// <summary>
    /// Validates that the sale belongs to the correct customer's cycle
    /// </summary>
    public void ValidateCustomerMatch(int cycleCustomerId)
    {
        if (CustomerId != cycleCustomerId)
            throw new InvalidOperationException("Sale customer must match cycle customer");
    }

    /// <summary>
    /// Updates sale details (before cycle is settled)
    /// </summary>
    public void Update(
        decimal quantity,
        decimal unitPrice,
        string? notes = null)
    {
        if (quantity <= 0)
            throw new ArgumentException("Quantity must be positive", nameof(quantity));

        if (unitPrice < 0)
            throw new ArgumentException("Unit price cannot be negative", nameof(unitPrice));

        _quantity = quantity;
        _unitPrice = unitPrice;
        _totalAmount = quantity * unitPrice;
        Notes = notes;
    }

    public override string ToString()
    {
        return $"Sale {SaleId}: {Quantity} @ {UnitPrice} = {TotalAmount}";
    }
}
