using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// MilkCycle repository implementation
/// </summary>
public class MilkCycleRepository : Repository<MilkCycle, int>, IMilkCycleRepository
{
    public MilkCycleRepository(DairyDbContext context) : base(context)
    {
    }

    public async Task<IEnumerable<MilkCycle>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(mc => mc.CustomerId == customerId)
            .OrderByDescending(mc => mc.CycleStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MilkCycle>> GetUnsettledCyclesByCustomerAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(mc => mc.CustomerId == customerId && !mc.IsSettled)
            .OrderBy(mc => mc.CycleStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MilkCycle>> GetAllUnsettledCyclesAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(mc => mc.Customer)
            .Where(mc => !mc.IsSettled)
            .OrderBy(mc => mc.CycleStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<MilkCycle>> GetCyclesByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(mc => mc.CycleStartDate >= startDate && mc.CycleEndDate <= endDate)
            .OrderBy(mc => mc.CycleStartDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<MilkCycle?> GetByIdWithDetailsAsync(int cycleId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(mc => mc.Customer)
            .Include(mc => mc.ProductSales)
                .ThenInclude(ps => ps.Product)
            .Include(mc => mc.AdvancePayments)
            .Include(mc => mc.Settlement)
            .FirstOrDefaultAsync(mc => mc.CycleId == cycleId, cancellationToken);
    }
}
