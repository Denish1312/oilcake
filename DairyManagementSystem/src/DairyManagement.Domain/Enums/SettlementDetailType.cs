namespace DairyManagement.Domain.Enums;

/// <summary>
/// Types of settlement detail line items
/// </summary>
public enum SettlementDetailType
{
    /// <summary>
    /// Milk amount (credit to customer)
    /// </summary>
    Milk = 1,
    
    /// <summary>
    /// Product sale (debit from customer)
    /// </summary>
    ProductSale = 2,
    
    /// <summary>
    /// Advance payment (debit from customer)
    /// </summary>
    Advance = 3
}
