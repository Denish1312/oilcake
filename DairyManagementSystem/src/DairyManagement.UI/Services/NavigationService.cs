using DairyManagement.UI.ViewModels;

namespace DairyManagement.UI.Services;

/// <summary>
/// Service for navigating between views
/// </summary>
public interface INavigationService
{
    /// <summary>
    /// Current view model being displayed
    /// </summary>
    BaseViewModel? CurrentViewModel { get; }

    /// <summary>
    /// Event raised when navigation occurs
    /// </summary>
    event Action<BaseViewModel?>? CurrentViewModelChanged;

    /// <summary>
    /// Navigates to a view model
    /// </summary>
    TViewModel NavigateTo<TViewModel>() where TViewModel : BaseViewModel;

    /// <summary>
    /// Navigates to a view model by type
    /// </summary>
    BaseViewModel NavigateTo(Type viewModelType);
}

/// <summary>
/// Navigation service implementation
/// </summary>
public class NavigationService : INavigationService
{
    private readonly Func<Type, BaseViewModel> _viewModelFactory;
    private BaseViewModel? _currentViewModel;

    public NavigationService(Func<Type, BaseViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public BaseViewModel? CurrentViewModel
    {
        get => _currentViewModel;
        private set
        {
            _currentViewModel = value;
            CurrentViewModelChanged?.Invoke(_currentViewModel);
        }
    }

    public event Action<BaseViewModel?>? CurrentViewModelChanged;

    public TViewModel NavigateTo<TViewModel>() where TViewModel : BaseViewModel
    {
        return (TViewModel)NavigateTo(typeof(TViewModel));
    }

    public BaseViewModel NavigateTo(Type viewModelType)
    {
        var viewModel = _viewModelFactory(viewModelType);
        CurrentViewModel = viewModel;
        return viewModel;
    }
}
