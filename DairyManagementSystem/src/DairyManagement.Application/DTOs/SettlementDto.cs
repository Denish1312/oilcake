namespace DairyManagement.Application.DTOs;

/// <summary>
/// DTO for Settlement calculation result
/// </summary>
public class SettlementDto
{
    public int SettlementId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public int CycleId { get; set; }
    public DateTime CycleStartDate { get; set; }
    public DateTime CycleEndDate { get; set; }
    public DateTime SettlementDate { get; set; }
    
    // Settlement calculation
    public decimal MilkAmount { get; set; }
    public decimal TotalProductSales { get; set; }
    public decimal TotalAdvancePaid { get; set; }
    public decimal FinalPayable { get; set; }
    
    // Payment details
    public string PaymentMode { get; set; } = string.Empty;
    public string? PaymentReference { get; set; }
    public DateTime? PaymentDate { get; set; }
    public bool IsPaid { get; set; }
    
    // Additional info
    public bool CustomerOwesMoney { get; set; }
    public List<SettlementDetailDto> Details { get; set; } = new();
}

/// <summary>
/// DTO for Settlement detail line item
/// </summary>
public class SettlementDetailDto
{
    public int DetailId { get; set; }
    public string DetailType { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public DateTime TransactionDate { get; set; }
    public bool IsCredit { get; set; }
}

/// <summary>
/// DTO for creating a settlement
/// </summary>
public class CreateSettlementDto
{
    public int CycleId { get; set; }
    public string PaymentMode { get; set; } = "CASH";
    public string? Notes { get; set; }
}
