using DairyManagement.Domain.Entities;

namespace DairyManagement.Infrastructure.Repositories;

public interface IUserRepository : IRepository<User, int>
{
    Task<User?> GetByUsernameAsync(string username, CancellationToken cancellationToken = default);
    Task<bool> UsernameExistsAsync(string username, CancellationToken cancellationToken = default);
}
