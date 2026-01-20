using DairyManagement.Domain.ValueObjects;

namespace DairyManagement.Domain.Entities;

/// <summary>
/// Represents a milk supplier (customer)
/// </summary>
public class Customer : BaseEntity
{
    /// <summary>
    /// Unique identifier
    /// </summary>
    public int CustomerId { get; private set; }

    /// <summary>
    /// Unique customer code (e.g., CUST001)
    /// </summary>
    public string CustomerCode { get; private set; } = string.Empty;

    /// <summary>
    /// Full name of the customer
    /// </summary>
    public string FullName { get; private set; } = string.Empty;

    /// <summary>
    /// Phone number (optional)
    /// </summary>
    public string? PhoneNumber { get; private set; }

    /// <summary>
    /// Address (optional)
    /// </summary>
    public string? Address { get; private set; }

    /// <summary>
    /// Village name (optional)
    /// </summary>
    public string? Village { get; private set; }

    /// <summary>
    /// Whether the customer is currently active
    /// </summary>
    public bool IsActive { get; private set; }

    // Navigation properties
    private readonly List<MilkCycle> _milkCycles = new();
    public IReadOnlyCollection<MilkCycle> MilkCycles => _milkCycles.AsReadOnly();

    /// <summary>
    /// Private constructor for EF Core
    /// </summary>
    private Customer() { }

    /// <summary>
    /// Creates a new customer
    /// </summary>
    public static Customer Create(
        string customerCode,
        string fullName,
        string? phoneNumber = null,
        string? address = null,
        string? village = null)
    {
        // Validation
        if (string.IsNullOrWhiteSpace(customerCode))
            throw new ArgumentException("Customer code is required", nameof(customerCode));

        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required", nameof(fullName));

        if (customerCode.Length > 20)
            throw new ArgumentException("Customer code cannot exceed 20 characters", nameof(customerCode));

        if (fullName.Length > 200)
            throw new ArgumentException("Full name cannot exceed 200 characters", nameof(fullName));

        return new Customer
        {
            CustomerCode = customerCode.ToUpperInvariant(),
            FullName = fullName,
            PhoneNumber = phoneNumber,
            Address = address,
            Village = village,
            IsActive = true
        };
    }

    /// <summary>
    /// Updates customer information
    /// </summary>
    public void Update(
        string fullName,
        string? phoneNumber = null,
        string? address = null,
        string? village = null)
    {
        if (string.IsNullOrWhiteSpace(fullName))
            throw new ArgumentException("Full name is required", nameof(fullName));

        if (fullName.Length > 200)
            throw new ArgumentException("Full name cannot exceed 200 characters", nameof(fullName));

        FullName = fullName;
        PhoneNumber = phoneNumber;
        Address = address;
        Village = village;
    }

    /// <summary>
    /// Deactivates the customer
    /// </summary>
    public void Deactivate()
    {
        IsActive = false;
    }

    /// <summary>
    /// Reactivates the customer
    /// </summary>
    public void Activate()
    {
        IsActive = true;
    }

    /// <summary>
    /// Checks if customer can have new milk cycles
    /// </summary>
    public bool CanCreateNewCycle()
    {
        return IsActive;
    }

    public override string ToString()
    {
        return $"{CustomerCode} - {FullName}";
    }
}
