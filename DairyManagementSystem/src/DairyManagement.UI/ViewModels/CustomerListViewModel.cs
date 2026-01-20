using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

/// <summary>
/// ViewModel for Customer list view
/// </summary>
public class CustomerListViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<CustomerDto> _customers = new();
    private CustomerDto? _selectedCustomer;
    private string _searchText = string.Empty;
    private bool _showActiveOnly = true;

    public CustomerListViewModel(
        ICustomerService customerService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _customerService = customerService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        // Initialize commands
        LoadCustomersCommand = new AsyncRelayCommand(LoadCustomersAsync);
        AddCustomerCommand = new RelayCommand(AddCustomer);
        EditCustomerCommand = new RelayCommand(EditCustomer, () => SelectedCustomer != null);
        DeleteCustomerCommand = new AsyncRelayCommand(DeleteCustomerAsync, () => SelectedCustomer != null);
        ActivateCustomerCommand = new AsyncRelayCommand(ActivateCustomerAsync, () => SelectedCustomer != null && !SelectedCustomer.IsActive);
        DeactivateCustomerCommand = new AsyncRelayCommand(DeactivateCustomerAsync, () => SelectedCustomer != null && SelectedCustomer.IsActive);
        SearchCommand = new AsyncRelayCommand(SearchCustomersAsync);

        // Load customers on initialization
        _ = LoadCustomersAsync();
    }

    public ObservableCollection<CustomerDto> Customers
    {
        get => _customers;
        set => SetProperty(ref _customers, value);
    }

    public CustomerDto? SelectedCustomer
    {
        get => _selectedCustomer;
        set
        {
            if (SetProperty(ref _selectedCustomer, value))
            {
                ((RelayCommand)EditCustomerCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeleteCustomerCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)ActivateCustomerCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeactivateCustomerCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string SearchText
    {
        get => _searchText;
        set => SetProperty(ref _searchText, value);
    }

    public bool ShowActiveOnly
    {
        get => _showActiveOnly;
        set
        {
            if (SetProperty(ref _showActiveOnly, value))
            {
                _ = LoadCustomersAsync();
            }
        }
    }

    // Commands
    public ICommand LoadCustomersCommand { get; }
    public ICommand AddCustomerCommand { get; }
    public ICommand EditCustomerCommand { get; }
    public ICommand DeleteCustomerCommand { get; }
    public ICommand ActivateCustomerCommand { get; }
    public ICommand DeactivateCustomerCommand { get; }
    public ICommand SearchCommand { get; }

    private async Task LoadCustomersAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            var customers = ShowActiveOnly
                ? await _customerService.GetActiveCustomersAsync()
                : await _customerService.GetAllCustomersAsync();

            Customers = new ObservableCollection<CustomerDto>(customers);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load customers: {ex.Message}");
            _dialogService.ShowError($"Failed to load customers: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task SearchCustomersAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchText))
        {
            await LoadCustomersAsync();
            return;
        }

        try
        {
            IsLoading = true;
            ClearError();

            var customers = await _customerService.SearchCustomersByNameAsync(SearchText);
            Customers = new ObservableCollection<CustomerDto>(customers);
        }
        catch (Exception ex)
        {
            SetError($"Search failed: {ex.Message}");
            _dialogService.ShowError($"Search failed: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddCustomer()
    {
        _navigationService.NavigateTo<CustomerFormViewModel>();
    }

    private void EditCustomer()
    {
        if (SelectedCustomer == null) return;
        
        // TODO: Pass customer ID to form
        _navigationService.NavigateTo<CustomerFormViewModel>();
    }

    private async Task DeleteCustomerAsync()
    {
        if (SelectedCustomer == null) return;

        var confirm = _dialogService.ShowConfirmation(
            $"Are you sure you want to deactivate customer '{SelectedCustomer.FullName}'?",
            "Confirm Deactivation");

        if (!confirm) return;

        try
        {
            IsLoading = true;
            await _customerService.DeactivateCustomerAsync(SelectedCustomer.CustomerId, "Admin");
            _dialogService.ShowMessage("Customer deactivated successfully");
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to deactivate customer: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task ActivateCustomerAsync()
    {
        if (SelectedCustomer == null) return;

        try
        {
            IsLoading = true;
            await _customerService.ActivateCustomerAsync(SelectedCustomer.CustomerId, "Admin");
            _dialogService.ShowMessage("Customer activated successfully");
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to activate customer: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeactivateCustomerAsync()
    {
        if (SelectedCustomer == null) return;

        var confirm = _dialogService.ShowConfirmation(
            $"Are you sure you want to deactivate customer '{SelectedCustomer.FullName}'?",
            "Confirm Deactivation");

        if (!confirm) return;

        try
        {
            IsLoading = true;
            await _customerService.DeactivateCustomerAsync(SelectedCustomer.CustomerId, "Admin");
            _dialogService.ShowMessage("Customer deactivated successfully");
            await LoadCustomersAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to deactivate customer: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
