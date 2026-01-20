using DairyManagement.Application.DTOs;

namespace DairyManagement.Application.Interfaces;

/// <summary>
/// Service interface for settlement operations
/// </summary>
public interface ISettlementService
{
    /// <summary>
    /// Calculates and creates a settlement for a milk cycle
    /// </summary>
    Task<SettlementDto> CreateSettlementAsync(CreateSettlementDto dto, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a settlement by ID with all details
    /// </summary>
    Task<SettlementDto?> GetSettlementByIdAsync(int settlementId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets settlement by cycle ID
    /// </summary>
    Task<SettlementDto?> GetSettlementByCycleIdAsync(int cycleId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all settlements for a customer
    /// </summary>
    Task<IEnumerable<SettlementDto>> GetCustomerSettlementsAsync(int customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets all unpaid settlements
    /// </summary>
    Task<IEnumerable<SettlementDto>> GetUnpaidSettlementsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Marks a settlement as paid
    /// </summary>
    Task MarkAsPaidAsync(int settlementId, string? paymentReference, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Previews settlement calculation without creating it
    /// </summary>
    Task<SettlementDto> PreviewSettlementAsync(int cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Records an advance payment for a customer cycle
    /// </summary>
    Task RecordAdvancePaymentAsync(int customerId, int cycleId, decimal amount, string username);

    /// <summary>
    /// Gets settlements within a date range
    /// </summary>
    Task<IEnumerable<SettlementDto>> GetSettlementsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
