namespace DairyManagement.Application.Interfaces;

public interface IAuthService
{
    Task<bool> LoginAsync(string username, string password);
    Task<bool> CreateUserAsync(string username, string password, string role);
    Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword);
    string? CurrentUser { get; }
    string? CurrentRole { get; }
    bool IsAuthenticated { get; }
}
