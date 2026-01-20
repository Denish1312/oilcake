using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Settlement repository implementation
/// </summary>
public class SettlementRepository : Repository<Settlement, int>, ISettlementRepository
{
    public SettlementRepository(DairyDbContext context) : base(context)
    {
    }

    public async Task<Settlement?> GetByCycleIdAsync(int cycleId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(s => s.CycleId == cycleId, cancellationToken);
    }

    public async Task<IEnumerable<Settlement>> GetByCustomerIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(s => s.CustomerId == customerId)
            .OrderByDescending(s => s.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<Settlement?> GetByIdWithDetailsAsync(int settlementId, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Customer)
            .Include(s => s.MilkCycle)
            .Include(s => s.Details)
            .FirstOrDefaultAsync(s => s.SettlementId == settlementId, cancellationToken);
    }

    public async Task<IEnumerable<Settlement>> GetUnpaidSettlementsAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Customer)
            .Where(s => !s.IsPaid)
            .OrderBy(s => s.SettlementDate)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Settlement>> GetSettlementsByDateRangeAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Include(s => s.Customer)
            .Where(s => s.SettlementDate >= startDate && s.SettlementDate <= endDate)
            .OrderBy(s => s.SettlementDate)
            .ToListAsync(cancellationToken);
    }
}
