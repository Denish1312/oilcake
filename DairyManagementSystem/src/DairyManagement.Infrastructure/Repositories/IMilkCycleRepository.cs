using DairyManagement.Domain.Entities;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Repository interface for MilkCycle entity
/// </summary>
public interface IMilkCycleRepository : IRepository<MilkCycle, int>
{
    /// <summary>
    /// Gets all milk cycles for a customer
    /// </summary>
    Task<IEnumerable<MilkCycle>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets unsettled cycles for a customer
    /// </summary>
    Task<IEnumerable<MilkCycle>> GetUnsettledCyclesByCustomerAsync(int customerId, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all unsettled cycles
    /// </summary>
    Task<IEnumerable<MilkCycle>> GetAllUnsettledCyclesAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets cycles within a date range
    /// </summary>
    Task<IEnumerable<MilkCycle>> GetCyclesByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets a cycle with all related data (sales, advances)
    /// </summary>
    Task<MilkCycle?> GetByIdWithDetailsAsync(int cycleId, CancellationToken cancellationToken = default);
}
