using DairyManagement.Application.DTOs;

namespace DairyManagement.Application.Interfaces;

/// <summary>
/// Service interface for customer operations
/// </summary>
public interface ICustomerService
{
    /// <summary>
    /// Gets all customers
    /// </summary>
    Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets active customers only
    /// </summary>
    Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a customer by ID
    /// </summary>
    Task<CustomerDto?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a customer by customer code
    /// </summary>
    Task<CustomerDto?> GetCustomerByCodeAsync(string customerCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Searches customers by name
    /// </summary>
    Task<IEnumerable<CustomerDto>> SearchCustomersByNameAsync(string searchTerm, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new customer
    /// </summary>
    Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing customer
    /// </summary>
    Task UpdateCustomerAsync(int customerId, UpdateCustomerDto dto, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Activates a customer
    /// </summary>
    Task ActivateCustomerAsync(int customerId, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deactivates a customer
    /// </summary>
    Task DeactivateCustomerAsync(int customerId, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a customer code already exists
    /// </summary>
    Task<bool> CustomerCodeExistsAsync(string customerCode, CancellationToken cancellationToken = default);
}
