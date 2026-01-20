using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Customer repository implementation
/// </summary>
public class CustomerRepository : Repository<Customer, int>, ICustomerRepository
{
    public CustomerRepository(DairyDbContext context) : base(context)
    {
    }

    public async Task<Customer?> GetByCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .FirstOrDefaultAsync(c => c.CustomerCode == customerCode.ToUpperInvariant(), cancellationToken);
    }

    public async Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => c.IsActive)
            .OrderBy(c => c.CustomerCode)
            .ToListAsync(cancellationToken);
    }

    public async Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .Where(c => EF.Functions.Like(c.FullName, $"%{searchTerm}%"))
            .OrderBy(c => c.FullName)
            .ToListAsync(cancellationToken);
    }

    public async Task<bool> CodeExistsAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AnyAsync(c => c.CustomerCode == customerCode.ToUpperInvariant(), cancellationToken);
    }
}
