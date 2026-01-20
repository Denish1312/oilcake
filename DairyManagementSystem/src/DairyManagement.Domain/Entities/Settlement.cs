using DairyManagement.Domain.Enums;
using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents a final settlement for a milk cycle
/// Calculates: Milk Amount - Product Sales - Advances = Final Payable
/// </summary>
public class Settlement : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int SettlementId { get; private set; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// Milk cycle ID (one settlement per cycle)
    /// </summary>
    public int CycleId { get; private set; }

    /// <summary>
    /// Settlement date and time
    /// </summary>
    public DateTime SettlementDate { get; private set; }

    // Settlement calculation components
    private decimal _milkAmount;
    public Money MilkAmount => Money.FromAmount(_milkAmount);

    private decimal _totalProductSales;
    public Money TotalProductSales => Money.FromAmount(_totalProductSales);

    private decimal _totalAdvancePaid;
    public Money TotalAdvancePaid => Money.FromAmount(_totalAdvancePaid);

    private decimal _finalPayable;
    public Money FinalPayable => Money.FromBalance(_finalPayable); // Can be negative

    // Payment details
    public PaymentMode PaymentMode { get; private set; }
    public string? PaymentReference { get; private set; }
    public DateTime? PaymentDate { get; private set; }
    public bool IsPaid { get; private set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public MilkCycle MilkCycle { get; private set; } = null!;
    
    private readonly List<SettlementDetail> _details = new();
    public IReadOnlyCollection<SettlementDetail> Details => _details.AsReadOnly();

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Settlement() { }

    /// <summary>
    /// Creates a new settlement
    /// </summary>
    public static Settlement Create(
        int customerId,
        int cycleId,
        decimal milkAmount,
        decimal totalProductSales,
        decimal totalAdvancePaid,
        PaymentMode paymentMode,
        string? notes = null)
    {
        // Validation
        if (milkAmount < 0)
            throw new ArgumentException("Milk amount cannot be negative", nameof(milkAmount));

        if (totalProductSales < 0)
            throw new ArgumentException("Total product sales cannot be negative", nameof(totalProductSales));

        if (totalAdvancePaid < 0)
            throw new ArgumentException("Total advance paid cannot be negative", nameof(totalAdvancePaid));

        // Calculate final payable (can be negative if customer owes money)
        var finalPayable = milkAmount - totalProductSales - totalAdvancePaid;

        return new Settlement
        {
            CustomerId = customerId,
            CycleId = cycleId,
            SettlementDate = DateTime.UtcNow,
            _milkAmount = milkAmount,
            _totalProductSales = totalProductSales,
            _totalAdvancePaid = totalAdvancePaid,
            _finalPayable = finalPayable,
            PaymentMode = paymentMode,
            IsPaid = false,
            Notes = notes
        };
    }

    /// <summary>
    /// Adds a settlement detail line item
    /// </summary>
    public void AddDetail(SettlementDetail detail)
    {
        _details.Add(detail);
    }

    /// <summary>
    /// Marks the settlement as paid
    /// </summary>
    public void MarkAsPaid(string? paymentReference = null)
    {
        if (IsPaid)
            throw new InvalidOperationException("Settlement is already marked as paid");

        IsPaid = true;
        PaymentDate = DateTime.UtcNow;
        PaymentReference = paymentReference;
    }

    /// <summary>
    /// Checks if customer owes money (negative balance)
    /// </summary>
    public bool CustomerOwesMoney()
    {
        return _finalPayable < 0;
    }

    /// <summary>
    /// Gets the absolute amount owed by customer (if negative balance)
    /// </summary>
    public Money GetAmountOwed()
    {
        return CustomerOwesMoney() 
            ? Money.FromAmount(Math.Abs(_finalPayable)) 
            : Money.Zero;
    }

    /// <summary>
    /// Checks if settlement requires payment to customer
    /// </summary>
    public bool RequiresPaymentToCustomer()
    {
        return _finalPayable > 0;
    }

    /// <summary>
    /// Validates settlement calculation
    /// </summary>
    public void ValidateCalculation()
    {
        var calculated = _milkAmount - _totalProductSales - _totalAdvancePaid;
        if (Math.Abs(calculated - _finalPayable) > 0.01m) // Allow 1 paisa tolerance
            throw new InvalidOperationException("Settlement calculation mismatch");
    }

    public override string ToString()
    {
        return $"Settlement {SettlementId}: {FinalPayable} ({(IsPaid ? "Paid" : "Unpaid")})";
    }
}
