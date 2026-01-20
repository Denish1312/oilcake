namespace DairyManagement.Application.DTOs;

public class MilkCycleDto
{
    public int CycleId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime CycleStartDate { get; set; }
    public DateTime CycleEndDate { get; set; }
    public decimal TotalMilkAmount { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettlementDate { get; set; }
}
