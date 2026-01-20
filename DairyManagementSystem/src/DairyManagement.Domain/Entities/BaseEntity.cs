namespace DairyManagement.Domain.Entities;

/// <summary>
/// Base class for all domain entities
/// Provides common audit fields and identity
/// </summary>
public abstract class BaseEntity
{
    /// <summary>
    /// When the entity was created
    /// </summary>
    public DateTime CreatedAt { get; protected set; }

    /// <summary>
    /// When the entity was last updated
    /// </summary>
    public DateTime UpdatedAt { get; protected set; }

    /// <summary>
    /// Username who created the entity
    /// </summary>
    public string? CreatedBy { get; protected set; }

    /// <summary>
    /// Username who last updated the entity
    /// </summary>
    public string? UpdatedBy { get; protected set; }

    /// <summary>
    /// Constructor for new entities
    /// </summary>
    protected BaseEntity()
    {
        CreatedAt = DateTime.UtcNow;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Sets the created by username
    /// </summary>
    public void SetCreatedBy(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        CreatedBy = username;
    }

    /// <summary>
    /// Updates the entity and sets updated by username
    /// </summary>
    public void SetUpdatedBy(string username)
    {
        if (string.IsNullOrWhiteSpace(username))
            throw new ArgumentException("Username cannot be empty", nameof(username));

        UpdatedBy = username;
        UpdatedAt = DateTime.UtcNow;
    }
}
