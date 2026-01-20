using System.ComponentModel.DataAnnotations;

namespace DairyManagement.Domain.Entities;

public class User : BaseEntity
{
    public int UserId { get; private set; }
    
    [Required]
    public string Username { get; private set; } = string.Empty;
    
    [Required]
    public string PasswordHash { get; private set; } = string.Empty;
    
    public string Role { get; private set; } = "Staff"; // Admin or Staff
    
    public bool IsActive { get; private set; } = true;

    private User() { } // EF Core

    public static User Create(string username, string passwordHash, string role = "Staff")
    {
        return new User
        {
            Username = username,
            PasswordHash = passwordHash,
            Role = role
        };
    }

    public void UpdatePassword(string newHash)
    {
        PasswordHash = newHash;
    }

    public void Deactivate() => IsActive = false;
    public void Activate() => IsActive = true;
}
