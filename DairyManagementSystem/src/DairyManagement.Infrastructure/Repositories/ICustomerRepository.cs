using DairyManagement.Domain.Entities;

namespace DairyManagement.Infrastructure.Repositories;

/// <summary>
/// Repository interface for Customer entity
/// </summary>
public interface ICustomerRepository : IRepository<Customer, int>
{
    /// <summary>
    /// Gets a customer by customer code
    /// </summary>
    Task<Customer?> GetByCodeAsync(string customerCode, CancellationToken cancellationToken = default);

    /// <summary>
    /// Gets all active customers
    /// </summary>
    Task<IEnumerable<Customer>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);

    /// <summary>
    /// Searches customers by name
    /// </summary>
    Task<IEnumerable<Customer>> SearchByNameAsync(string searchTerm, CancellationToken cancellationToken = default);

    /// <summary>
    /// Checks if a customer code already exists
    /// </summary>
    Task<bool> CodeExistsAsync(string customerCode, CancellationToken cancellationToken = default);
}
