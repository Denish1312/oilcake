using DairyManagement.Domain.Entities;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Settlement entity
/// </summary>
public interface ISettlementRepository : IRepository<Settlement, int>
{
    /// <summary>
    /// Gets settlement by cycle ID
    /// </summary>
    Task<Settlement?> GetByCycleIdAsync(int cycleId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all settlements for a customer
    /// </summary>
    Task<IEnumerable<Settlement>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets settlement with all details
    /// </summary>
    Task<Settlement?> GetByIdWithDetailsAsync(int settlementId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unpaid settlements
    /// </summary>
    Task<IEnumerable<Settlement>> GetUnpaidSettlementsAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets settlements within a date range
    /// </summary>
    Task<IEnumerable<Settlement>> GetSettlementsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);
}
