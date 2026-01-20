using DairyManagement.Application.DTOs;

namespace DairyManagement.Application.Interfaces;

public interface IReceiptService
{
    /// <summary>
    /// Generates a formatted string for a thermal printer receipt (ESC/POS compatible)
    /// </summary>
    string GenerateSettlementReceipt(SettlementDto settlement);
}
