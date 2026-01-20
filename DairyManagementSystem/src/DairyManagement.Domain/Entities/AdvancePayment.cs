using DairyManagement.Domain.Enums;
using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents an advance payment given to a customer during a milk cycle
/// </summary>
public class AdvancePayment : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int AdvanceId { get; private set; }

    /// <summary>
    /// Customer ID
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// Milk cycle ID
    /// </summary>
    public int CycleId { get; private set; }

    /// <summary>
    /// Payment date and time
    /// </summary>
    public DateTime PaymentDate { get; private set; }

    /// <summary>
    /// Advance amount
    /// </summary>
    private decimal _amount;
    public Money Amount => Money.FromAmount(_amount);

    /// <summary>
    /// Payment mode (Cash, Bank Transfer, etc.)
    /// </summary>
    public PaymentMode PaymentMode { get; private set; }

    /// <summary>
    /// Reference number (for non-cash payments)
    /// </summary>
    public string? ReferenceNumber { get; private set; }

    /// <summary>
    /// Optional notes
    /// </summary>
    public string? Notes { get; private set; }

    // Navigation properties
    public Customer Customer { get; private set; } = null!;
    public MilkCycle MilkCycle { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private AdvancePayment() { }

    /// <summary>
    /// Creates a new advance payment
    /// </summary>
    public static AdvancePayment Create(
        int customerId,
        int cycleId,
        decimal amount,
        PaymentMode paymentMode,
        string? referenceNumber = null,
        string? notes = null)
    {
        // Validation
        if (amount <= 0)
            throw new ArgumentException("Advance amount must be positive", nameof(amount));

        // Reference number required for non-cash payments
        if (paymentMode != PaymentMode.Cash && string.IsNullOrWhiteSpace(referenceNumber))
            throw new ArgumentException("Reference number is required for non-cash payments", nameof(referenceNumber));

        return new AdvancePayment
        {
            CustomerId = customerId,
            CycleId = cycleId,
            PaymentDate = DateTime.UtcNow,
            _amount = amount,
            PaymentMode = paymentMode,
            ReferenceNumber = referenceNumber,
            Notes = notes
        };
    }

    /// <summary>
    /// Validates that the advance belongs to the correct customer's cycle
    /// </summary>
    public void ValidateCustomerMatch(int cycleCustomerId)
    {
        if (CustomerId != cycleCustomerId)
            throw new InvalidOperationException("Advance customer must match cycle customer");
    }

    /// <summary>
    /// Updates advance details (before cycle is settled)
    /// </summary>
    public void Update(
        decimal amount,
        PaymentMode paymentMode,
        string? referenceNumber = null,
        string? notes = null)
    {
        if (amount <= 0)
            throw new ArgumentException("Advance amount must be positive", nameof(amount));

        if (paymentMode != PaymentMode.Cash && string.IsNullOrWhiteSpace(referenceNumber))
            throw new ArgumentException("Reference number is required for non-cash payments", nameof(referenceNumber));

        _amount = amount;
        PaymentMode = paymentMode;
        ReferenceNumber = referenceNumber;
        Notes = notes;
    }

    public override string ToString()
    {
        return $"Advance {AdvanceId}: {Amount} ({PaymentMode})";
    }
}
