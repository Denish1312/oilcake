using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class UserManagementViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<User> _users = new();
    private string _newUsername = string.Empty;
    private string _newPassword = string.Empty;
    private string _newRole = "Staff";

    public UserManagementViewModel(
        IAuthService authService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _authService = authService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        CreateUserCommand = new AsyncRelayCommand(CreateUserAsync, CanCreateUser);
        
        // Note: In a real app we'd load existing users here.
        // For now, focus on the creation flow as requested.
    }

    public string NewUsername
    {
        get => _newUsername;
        set { if (SetProperty(ref _newUsername, value)) ((AsyncRelayCommand)CreateUserCommand).RaiseCanExecuteChanged(); }
    }

    public string NewPassword
    {
        get => _newPassword;
        set { if (SetProperty(ref _newPassword, value)) ((AsyncRelayCommand)CreateUserCommand).RaiseCanExecuteChanged(); }
    }

    public string NewRole
    {
        get => _newRole;
        set => SetProperty(ref _newRole, value);
    }

    public ICommand CreateUserCommand { get; }

    private bool CanCreateUser() => !string.IsNullOrWhiteSpace(NewUsername) && !string.IsNullOrWhiteSpace(NewPassword);

    private async Task CreateUserAsync()
    {
        try
        {
            IsLoading = true;
            if (await _authService.CreateUserAsync(NewUsername, NewPassword, NewRole))
            {
                _dialogService.ShowMessage($"User '{NewUsername}' created successfully as {NewRole}.");
                NewUsername = string.Empty;
                NewPassword = string.Empty;
            }
            else
            {
                _dialogService.ShowError("Failed to create user. Username might already exist.");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error creating user: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
