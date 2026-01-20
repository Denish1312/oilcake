using DairyManagement.Application.DTOs;

namespace DairyManagement.Application.Interfaces;

public interface IMilkCycleService
{
    Task<IEnumerable<MilkCycleDto>> GetAllCyclesAsync(bool unsettledOnly = false, CancellationToken cancellationToken = default);
    Task<MilkCycleDto?> GetCycleByIdAsync(int cycleId, CancellationToken cancellationToken = default);
    Task<int> CreateCycleAsync(int customerId, DateTime startDate, DateTime endDate, decimal totalMilkAmount, string username, CancellationToken cancellationToken = default);
    Task MarkAsSettledAsync(int cycleId, string username, CancellationToken cancellationToken = default);
}
