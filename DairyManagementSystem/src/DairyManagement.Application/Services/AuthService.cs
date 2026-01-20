using System.Security.Cryptography;
using System.Text;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace DairyManagement.Application.Services;

public class AuthService : IAuthService
{
    private readonly IUnitOfWork _unitOfWork;
    private string? _currentUser;
    private string? _currentRole;

    public AuthService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public string? CurrentUser => _currentUser;
    public string? CurrentRole => _currentRole;
    public bool IsAuthenticated => _currentUser != null;

    public async Task<bool> LoginAsync(string username, string password)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (user == null || !user.IsActive) return false;

        var inputHash = HashPassword(password);
        if (user.PasswordHash == inputHash)
        {
            _currentUser = user.Username;
            _currentRole = user.Role;
            return true;
        }

        return false;
    }

    public async Task<bool> CreateUserAsync(string username, string password, string role)
    {
        if (await _unitOfWork.Users.UsernameExistsAsync(username))
            return false;

        var hash = HashPassword(password);
        var user = User.Create(username, hash, role);
        await _unitOfWork.Users.AddAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    public async Task<bool> ChangePasswordAsync(string username, string currentPassword, string newPassword)
    {
        var user = await _unitOfWork.Users.GetByUsernameAsync(username);
        if (user == null) return false;

        var currentHash = HashPassword(currentPassword);
        if (user.PasswordHash != currentHash) return false;

        user.UpdatePassword(HashPassword(newPassword));
        await _unitOfWork.Users.UpdateAsync(user);
        await _unitOfWork.SaveChangesAsync();
        return true;
    }

    private string HashPassword(string password)
    {
        using var sha256 = SHA256.Create();
        var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
    }
}
