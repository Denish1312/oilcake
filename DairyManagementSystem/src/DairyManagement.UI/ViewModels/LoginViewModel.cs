using System.Windows.Input;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class LoginViewModel : BaseViewModel
{
    private readonly IAuthService _authService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private string _username = string.Empty;
    private string _password = string.Empty;

    public LoginViewModel(
        IAuthService authService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _authService = authService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        LoginCommand = new AsyncRelayCommand(LoginAsync, () => !string.IsNullOrWhiteSpace(Username));
    }

    public string Username
    {
        get => _username;
        set => SetProperty(ref _username, value);
    }

    public string Password
    {
        get => _password;
        set => SetProperty(ref _password, value);
    }

    public ICommand LoginCommand { get; }

    private async Task LoginAsync()
    {
        try
        {
            IsLoading = true;
            if (await _authService.LoginAsync(Username, Password))
            {
                // Navigate to Dashboard
                _navigationService.NavigateTo<DashboardViewModel>();
            }
            else
            {
                _dialogService.ShowError("Invalid username or password.");
            }
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Login Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
