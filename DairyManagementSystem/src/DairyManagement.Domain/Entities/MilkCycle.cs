using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents a 10-day milk collection cycle for a customer
/// </summary>
public class MilkCycle : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int CycleId { get; private set; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// Cycle start date
    /// </summary>
    public DateTime CycleStartDate { get; private set; }

    /// <summary>
    /// Cycle end date
    /// </summary>
    public DateTime CycleEndDate { get; private set; }

    /// <summary>
    /// Date range for the cycle
    /// </summary>
    public DateRange DateRange => DateRange.Create(CycleStartDate, CycleEndDate);

    /// <summary>
    /// Total milk amount in ₹ for the entire cycle
    /// </summary>
    private decimal _totalMilkAmount;
    public Money TotalMilkAmount => Money.FromAmount(_totalMilkAmount);

    /// <summary>
    /// Whether the cycle has been settled
    /// </summary>
    public bool IsSettled { get; private set; }

    /// <summary>
    /// When the cycle was settled
    /// </summary>
    public DateTime? SettlementDate { get; private set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    
    private readonly List<ProductSale> _productSales = new();
    public IReadOnlyCollection<ProductSale> ProductSales => _productSales.AsReadOnly();
    
    private readonly List<AdvancePayment> _advancePayments = new();
    public IReadOnlyCollection<AdvancePayment> AdvancePayments => _advancePayments.AsReadOnly();

    public Settlement? Settlement { get; private set; }

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private MilkCycle() { }

    /// <summary>
    /// Creates a new milk cycle
    /// </summary>
    public static MilkCycle Create(
        int customerId,
        DateTime startDate,
        DateTime endDate,
        string? notes = null)
    {
        // Validation
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date", nameof(endDate));

        return new MilkCycle
        {
            CustomerId = customerId,
            CycleStartDate = startDate.Date,
            CycleEndDate = endDate.Date,
            _totalMilkAmount = 0,
            IsSettled = false,
            Notes = notes
        };
    }

    /// <summary>
    /// Creates a standard 10-day milk cycle
    /// </summary>
    public static MilkCycle CreateTenDayCycle(int customerId, DateTime startDate, string? notes = null)
    {
        return Create(customerId, startDate, startDate.AddDays(9), notes);
    }

    /// <summary>
    /// Sets the total milk amount for the cycle
    /// </summary>
    public void SetMilkAmount(decimal amount)
    {
        if (IsSettled)
            throw new InvalidOperationException("Cannot modify settled cycle");

        if (amount < 0)
            throw new ArgumentException("Milk amount cannot be negative", nameof(amount));

        _totalMilkAmount = amount;
    }

    /// <summary>
    /// Updates cycle notes
    /// </summary>
    public void UpdateNotes(string? notes)
    {
        if (IsSettled)
            throw new InvalidOperationException("Cannot modify settled cycle");

        Notes = notes;
    }

    /// <summary>
    /// Marks the cycle as settled
    /// </summary>
    public void MarkAsSettled()
    {
        if (IsSettled)
            throw new InvalidOperationException("Cycle is already settled");

        if (_totalMilkAmount == 0)
            throw new InvalidOperationException("Cannot settle cycle with zero milk amount");

        IsSettled = true;
        SettlementDate = DateTime.UtcNow;
    }

    /// <summary>
    /// Checks if the cycle can be settled
    /// </summary>
    public bool CanBeSettled()
    {
        return !IsSettled && _totalMilkAmount > 0;
    }

    /// <summary>
    /// Checks if the cycle has ended
    /// </summary>
    public bool HasEnded()
    {
        return CycleEndDate < DateTime.Today;
    }

    /// <summary>
    /// Checks if the cycle is currently active
    /// </summary>
    public bool IsActive()
    {
        var today = DateTime.Today;
        return today >= CycleStartDate && today <= CycleEndDate;
    }

    /// <summary>
    /// Calculates total product sales for this cycle
    /// </summary>
    public Money CalculateTotalProductSales()
    {
        var total = _productSales.Sum(ps => ps.TotalAmount.Amount);
        return Money.FromAmount(total);
    }

    /// <summary>
    /// Calculates total advances for this cycle
    /// </summary>
    public Money CalculateTotalAdvances()
    {
        var total = _advancePayments.Sum(ap => ap.Amount.Amount);
        return Money.FromAmount(total);
    }

    /// <summary>
    /// Calculates estimated payable (for preview before settlement)
    /// </summary>
    public Money CalculateEstimatedPayable()
    {
        return TotalMilkAmount - CalculateTotalProductSales() - CalculateTotalAdvances();
    }

    public override string ToString()
    {
        return $"Cycle {CycleId}: {CycleStartDate:dd/MM/yyyy} to {CycleEndDate:dd/MM/yyyy}";
    }
}
