using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Repositories;

namespace DairyManagement.Application.Services;

/// <summary>
/// Customer service implementation
/// </summary>
public class CustomerService : ICustomerService
{
    private readonly IUnitOfWork _unitOfWork;

    public CustomerService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<CustomerDto>> GetAllCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Customers.GetAllAsync(cancellationToken);
        return customers.Select(MapToDto);
    }

    public async Task<IEnumerable<CustomerDto>> GetActiveCustomersAsync(CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Customers.GetActiveCustomersAsync(cancellationToken);
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto?> GetCustomerByIdAsync(int customerId, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<CustomerDto?> GetCustomerByCodeAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByCodeAsync(customerCode, cancellationToken);
        return customer == null ? null : MapToDto(customer);
    }

    public async Task<IEnumerable<CustomerDto>> SearchCustomersByNameAsync(string searchTerm, CancellationToken cancellationToken = default)
    {
        var customers = await _unitOfWork.Customers.SearchByNameAsync(searchTerm, cancellationToken);
        return customers.Select(MapToDto);
    }

    public async Task<CustomerDto> CreateCustomerAsync(CreateCustomerDto dto, string username, CancellationToken cancellationToken = default)
    {
        // Check if code already exists
        if (await _unitOfWork.Customers.CodeExistsAsync(dto.CustomerCode, cancellationToken))
            throw new InvalidOperationException($"Customer code '{dto.CustomerCode}' already exists");

        // Create customer
        var customer = Customer.Create(
            dto.CustomerCode,
            dto.FullName,
            dto.PhoneNumber,
            dto.Address,
            dto.Village
        );
        customer.SetCreatedBy(username);

        // Save
        await _unitOfWork.Customers.AddAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(customer);
    }

    public async Task UpdateCustomerAsync(int customerId, UpdateCustomerDto dto, string username, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
        if (customer == null)
            throw new InvalidOperationException($"Customer {customerId} not found");

        // Update
        customer.Update(
            dto.FullName,
            dto.PhoneNumber,
            dto.Address,
            dto.Village
        );
        customer.SetUpdatedBy(username);

        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateCustomerAsync(int customerId, string username, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
        if (customer == null)
            throw new InvalidOperationException($"Customer {customerId} not found");

        customer.Activate();
        customer.SetUpdatedBy(username);

        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateCustomerAsync(int customerId, string username, CancellationToken cancellationToken = default)
    {
        var customer = await _unitOfWork.Customers.GetByIdAsync(customerId, cancellationToken);
        if (customer == null)
            throw new InvalidOperationException($"Customer {customerId} not found");

        customer.Deactivate();
        customer.SetUpdatedBy(username);

        await _unitOfWork.Customers.UpdateAsync(customer, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> CustomerCodeExistsAsync(string customerCode, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Customers.CodeExistsAsync(customerCode, cancellationToken);
    }

    private static CustomerDto MapToDto(Customer customer)
    {
        return new CustomerDto
        {
            CustomerId = customer.CustomerId,
            CustomerCode = customer.CustomerCode,
            FullName = customer.FullName,
            PhoneNumber = customer.PhoneNumber,
            Address = customer.Address,
            Village = customer.Village,
            IsActive = customer.IsActive,
            CreatedAt = customer.CreatedAt,
            UpdatedAt = customer.UpdatedAt
        };
    }
}
