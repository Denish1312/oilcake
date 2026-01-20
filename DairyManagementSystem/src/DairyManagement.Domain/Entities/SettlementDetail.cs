using DairyManagement.Domain.Enums;
using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents a line item in a settlement (for receipt printing)
/// </summary>
public class SettlementDetail : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int DetailId { get; private set; }

    /// <summary>
    /// Settlement ID
    /// </summary>
    public int SettlementId { get; private set; }

    /// <summary>
    /// Type of detail (Milk, ProductSale, Advance)
    /// </summary>
    public SettlementDetailType DetailType { get; private set; }

    /// <summary>
    /// Reference ID (links to ProductSale or AdvancePayment)
    /// Null for Milk entries
    /// </summary>
    public int? ReferenceId { get; private set; }

    /// <summary>
    /// Human-readable description
    /// </summary>
    public string Description { get; private set; } = string.Empty;

    /// <summary>
    /// Line item amount
    /// </summary>
    private decimal _amount;
    public Money Amount => Money.FromAmount(_amount);

    /// <summary>
    /// Date of the transaction (Milk collection, sale, or advance)
    /// </summary>
    public DateTime TransactionDate { get; private set; }

    // Navigation properties
    public Settlement Settlement { get; private set; } = null!;

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private SettlementDetail() { }

    /// <summary>
    /// Creates a milk detail (credit to customer)
    /// </summary>
    public static SettlementDetail CreateMilkDetail(
        int settlementId,
        decimal amount,
        DateTime transactionDate,
        string description = "Milk Amount")
    {
        if (amount < 0)
            throw new ArgumentException("Milk amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        return new SettlementDetail
        {
            SettlementId = settlementId,
            DetailType = SettlementDetailType.Milk,
            ReferenceId = null,
            Description = description,
            _amount = amount,
            TransactionDate = transactionDate
        };
    }

    /// <summary>
    /// Creates a product sale detail (debit from customer)
    /// </summary>
    public static SettlementDetail CreateProductSaleDetail(
        int settlementId,
        int saleId,
        string description,
        decimal amount,
        DateTime transactionDate)
    {
        if (amount < 0)
            throw new ArgumentException("Product sale amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        return new SettlementDetail
        {
            SettlementId = settlementId,
            DetailType = SettlementDetailType.ProductSale,
            ReferenceId = saleId,
            Description = description,
            _amount = amount,
            TransactionDate = transactionDate
        };
    }

    /// <summary>
    /// Creates an advance detail (debit from customer)
    /// </summary>
    public static SettlementDetail CreateAdvanceDetail(
        int settlementId,
        int advanceId,
        string description,
        decimal amount,
        DateTime transactionDate)
    {
        if (amount < 0)
            throw new ArgumentException("Advance amount cannot be negative", nameof(amount));

        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description is required", nameof(description));

        return new SettlementDetail
        {
            SettlementId = settlementId,
            DetailType = SettlementDetailType.Advance,
            ReferenceId = advanceId,
            Description = description,
            _amount = amount,
            TransactionDate = transactionDate
        };
    }

    /// <summary>
    /// Checks if this is a credit entry (adds to customer balance)
    /// </summary>
    public bool IsCredit()
    {
        return DetailType == SettlementDetailType.Milk;
    }

    /// <summary>
    /// Checks if this is a debit entry (subtracts from customer balance)
    /// </summary>
    public bool IsDebit()
    {
        return DetailType == SettlementDetailType.ProductSale || 
               DetailType == SettlementDetailType.Advance;
    }

    /// <summary>
    /// Gets the signed amount (positive for credit, negative for debit)
    /// </summary>
    public Money GetSignedAmount()
    {
        return IsCredit() 
            ? Money.FromAmount(_amount) 
            : Money.FromBalance(-_amount);
    }

    public override string ToString()
    {
        var sign = IsCredit() ? "+" : "-";
        return $"{DetailType}: {Description} {sign}₹{_amount:N2}";
    }
}
