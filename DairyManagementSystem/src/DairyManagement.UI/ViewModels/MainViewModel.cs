using System.Windows.Input;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;
using DairyManagement.Application.Interfaces;

namespace DairyManagement.UI.ViewModels;

/// <summary>
/// Main ViewModel for the application window
/// </summary>
public class MainViewModel : BaseViewModel
{
    private readonly INavigationService _navigationService;
    private readonly IAuthService _authService;
    private readonly ILicensingService _licensingService;
    private BaseViewModel? _currentViewModel;
    private string _statusMessage = "Ready";
    private string? _currentUser;
    private bool _isAuthenticated;
    private bool _isActivated;

    public MainViewModel(
        INavigationService navigationService,
        IAuthService authService,
        ILicensingService licensingService)
    {
        _navigationService = navigationService;
        _authService = authService;
        _licensingService = licensingService;

        // Subscribe to navigation changes
        _navigationService.CurrentViewModelChanged += OnCurrentViewModelChanged;

        // Initialize commands
        NavigateToDashboardCommand = new RelayCommand(() => _navigationService.NavigateTo<DashboardViewModel>());
        NavigateToCustomersCommand = new RelayCommand(() => _navigationService.NavigateTo<CustomerListViewModel>());
        NavigateToProductsCommand = new RelayCommand(() => _navigationService.NavigateTo<ProductListViewModel>());
        NavigateToMilkCyclesCommand = new RelayCommand(() => _navigationService.NavigateTo<MilkCycleListViewModel>());
        NavigateToSettlementsCommand = new RelayCommand(() => _navigationService.NavigateTo<MilkCycleListViewModel>()); 
        NavigateToReportsCommand = new RelayCommand(() => _navigationService.NavigateTo<ReportsViewModel>());
        NavigateToUserManagementCommand = new RelayCommand(() => _navigationService.NavigateTo<UserManagementViewModel>());
        LogoutCommand = new RelayCommand(Logout);
    }

    public bool IsAdmin => _authService.CurrentRole == "Admin";

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        private set => SetProperty(ref _currentViewModel, value);
    }

    public string StatusMessage
    {
        get => _statusMessage;
        set => SetProperty(ref _statusMessage, value);
    }

    public bool IsAuthenticated
    {
        get => _isAuthenticated;
        set => SetProperty(ref _isAuthenticated, value);
    }

    public bool IsActivated
    {
        get => _isActivated;
        set => SetProperty(ref _isActivated, value);
    }

    public string? CurrentUser
    {
        get => _currentUser;
        set => SetProperty(ref _currentUser, value);
    }

    // Commands
    public ICommand NavigateToDashboardCommand { get; }
    public ICommand NavigateToCustomersCommand { get; }
    public ICommand NavigateToProductsCommand { get; }
    public ICommand NavigateToMilkCyclesCommand { get; }
    public ICommand NavigateToSettlementsCommand { get; }
    public ICommand NavigateToReportsCommand { get; }
    public ICommand NavigateToUserManagementCommand { get; }
    public ICommand LogoutCommand { get; }

    private void Logout()
    {
        // Simple logout - actual auth service should handle state
        IsAuthenticated = false;
        CurrentUser = null;
        _navigationService.NavigateTo<LoginViewModel>();
    }

    private void OnCurrentViewModelChanged(BaseViewModel? viewModel)
    {
        CurrentViewModel = viewModel;
        
        // Update global states
        IsAuthenticated = _authService.IsAuthenticated;
        IsActivated = _licensingService.IsActivated();
        CurrentUser = _authService.CurrentUser;

        // Update status message based on current view
        StatusMessage = viewModel switch
        {
            DashboardViewModel => "Dashboard",
            CustomerListViewModel => "Customer Management",
            ProductListViewModel => "Product Management",
            UserManagementViewModel => "User Management",
            LoginViewModel => "Please Login",
            ActivationViewModel => "Software Activation Required",
            _ => "Ready"
        };
    }
}
