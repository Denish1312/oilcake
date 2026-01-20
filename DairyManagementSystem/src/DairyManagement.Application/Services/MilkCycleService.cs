using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.Domain.ValueObjects;
using DairyManagement.Infrastructure.Repositories;

namespace DairyManagement.Application.Services;

public class MilkCycleService : IMilkCycleService
{
    private readonly IUnitOfWork _unitOfWork;

    public MilkCycleService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<MilkCycleDto>> GetAllCyclesAsync(bool unsettledOnly = false, CancellationToken cancellationToken = default)
    {
        var cycles = await _unitOfWork.MilkCycles.GetAllAsync(cancellationToken);
        
        var query = cycles.Select(c => new MilkCycleDto
        {
            CycleId = c.CycleId,
            CustomerId = c.CustomerId,
            CustomerName = c.Customer?.FullName ?? "Unknown",
            CycleStartDate = c.CycleStartDate,
            CycleEndDate = c.CycleEndDate,
            TotalMilkAmount = c.TotalMilkAmount.Amount,
            IsSettled = c.IsSettled,
            SettlementDate = c.SettlementDate
        });

        if (unsettledOnly)
            query = query.Where(c => !c.IsSettled);

        return query.OrderByDescending(c => c.CycleStartDate).ToList();
    }

    public async Task<MilkCycleDto?> GetCycleByIdAsync(int cycleId, CancellationToken cancellationToken = default)
    {
        var c = await _unitOfWork.MilkCycles.GetByIdAsync(cycleId, cancellationToken);
        if (c == null) return null;

        return new MilkCycleDto
        {
            CycleId = c.CycleId,
            CustomerId = c.CustomerId,
            CustomerName = c.Customer?.FullName ?? "Unknown",
            CycleStartDate = c.CycleStartDate,
            CycleEndDate = c.CycleEndDate,
            TotalMilkAmount = c.TotalMilkAmount.Amount,
            IsSettled = c.IsSettled,
            SettlementDate = c.SettlementDate
        };
    }

    public async Task<int> CreateCycleAsync(int customerId, DateTime startDate, DateTime endDate, decimal totalMilkAmount, string username, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
        if (customer == null) throw new InvalidOperationException("Customer not found");

        var cycle = MilkCycle.Create(
            customerId,
            startDate,
            endDate
        );
        cycle.SetCreatedBy(username);

        await _unitOfWork.MilkCycles.AddAsync(cycle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return cycle.CycleId;
    }

    public async Task MarkAsSettledAsync(int cycleId, string username, CancellationToken cancellationToken = default)
    {
        var cycle = await _unitOfWork.MilkCycles.GetByIdAsync(cycleId, cancellationToken);
        if (cycle == null) throw new InvalidOperationException("Cycle not found");

        cycle.MarkAsSettled();
        cycle.SetUpdatedBy(username);

        await _unitOfWork.MilkCycles.UpdateAsync(cycle, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }
}
