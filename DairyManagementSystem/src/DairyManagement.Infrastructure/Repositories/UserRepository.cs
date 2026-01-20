using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace DairyManagement.Infrastructure.Repositories;

public class UserRepository : Repository<User, int>, IUserRepository
{
    public UserRepository(DairyDbContext context) : base(context)
    {
    }

    public async Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet.FirstOrDefaultAsync(u => u.Username == username, cancellationToken);
    }

    public async Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default)
    {
        return await DbSet.AnyAsync(u => u.Username == username, cancellationToken);
    }
}
