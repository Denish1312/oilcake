using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

/// <summary>
/// ViewModel for Customer form (Create/Edit)
/// </summary>
public class CustomerFormViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private int? _customerId;
    private string _customerCode = string.Empty;
    private string _fullName = string.Empty;
    private string _phoneNumber = string.Empty;
    private string _address = string.Empty;
    private string _village = string.Empty;
    private bool _isEditMode;

    public CustomerFormViewModel(
        ICustomerService customerService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _customerService = customerService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        // Initialize commands
        SaveCommand = new AsyncRelayCommand(SaveCustomerAsync, CanSave);
        CancelCommand = new RelayCommand(Cancel);
        GenerateCodeCommand = new AsyncRelayCommand(GenerateCustomerCodeAsync);

        // Generate code for new customers
        if (!IsEditMode)
        {
            _ = GenerateCustomerCodeAsync();
        }
    }

    public int? CustomerId
    {
        get => _customerId;
        set => SetProperty(ref _customerId, value);
    }

    public string CustomerCode
    {
        get => _customerCode;
        set => SetProperty(ref _customerCode, value);
    }

    public string FullName
    {
        get => _fullName;
        set
        {
            if (SetProperty(ref _fullName, value))
            {
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string PhoneNumber
    {
        get => _phoneNumber;
        set => SetProperty(ref _phoneNumber, value);
    }

    public string Address
    {
        get => _address;
        set => SetProperty(ref _address, value);
    }

    public string Village
    {
        get => _village;
        set => SetProperty(ref _village, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public string FormTitle => IsEditMode ? "Edit Customer" : "New Customer";

    // Commands
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand GenerateCodeCommand { get; }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(CustomerCode) &&
               !string.IsNullOrWhiteSpace(FullName);
    }

    private async Task GenerateCustomerCodeAsync()
    {
        try
        {
            // Simple auto-generation logic
            var customers = await _customerService.GetAllCustomersAsync();
            var maxCode = customers
                .Select(c => c.CustomerCode)
                .Where(code => code.StartsWith("CUST"))
                .Select(code => int.TryParse(code.Substring(4), out var num) ? num : 0)
                .DefaultIfEmpty(0)
                .Max();

            CustomerCode = $"CUST{(maxCode + 1):D3}";
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to generate customer code: {ex.Message}");
            CustomerCode = "CUST001";
        }
    }

    private async Task SaveCustomerAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            if (IsEditMode && CustomerId.HasValue)
            {
                // Update existing customer
                var updateDto = new UpdateCustomerDto
                {
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    Address = Address,
                    Village = Village
                };

                await _customerService.UpdateCustomerAsync(CustomerId.Value, updateDto, "Admin");
                _dialogService.ShowMessage("Customer updated successfully");
            }
            else
            {
                // Create new customer
                var createDto = new CreateCustomerDto
                {
                    CustomerCode = CustomerCode,
                    FullName = FullName,
                    PhoneNumber = PhoneNumber,
                    Address = Address,
                    Village = Village
                };

                await _customerService.CreateCustomerAsync(createDto, "Admin");
                _dialogService.ShowMessage("Customer created successfully");
            }

            // Navigate back to list
            _navigationService.NavigateTo<CustomerListViewModel>();
        }
        catch (Exception ex)
        {
            SetError($"Failed to save customer: {ex.Message}");
            _dialogService.ShowError($"Failed to save customer: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Cancel()
    {
        _navigationService.NavigateTo<CustomerListViewModel>();
    }

    public void LoadCustomer(int customerId)
    {
        // TODO: Load customer data for editing
        IsEditMode = true;
        CustomerId = customerId;
    }
}
