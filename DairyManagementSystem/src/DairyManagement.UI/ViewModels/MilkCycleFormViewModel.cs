using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.Domain.ValueObjects;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class MilkCycleFormViewModel : BaseViewModel
{
    private readonly IMilkCycleService _milkCycleService;
    private readonly ICustomerService _customerService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<CustomerDto> _customers = new();
    private CustomerDto? _selectedCustomer;
    private DateTime _startDate = DateTime.Now.Date;
    private DateTime _endDate = DateTime.Now.Date.AddDays(9);
    private decimal _totalMilkAmount;

    public MilkCycleFormViewModel(
        IMilkCycleService milkCycleService,
        ICustomerService customerService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _milkCycleService = milkCycleService;
        _customerService = customerService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new RelayCommand(Cancel);

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
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public DateTime StartDate
    {
        get => _startDate;
        set => SetProperty(ref _startDate, value);
    }

    public DateTime EndDate
    {
        get => _endDate;
        set => SetProperty(ref _endDate, value);
    }

    public decimal TotalMilkAmount
    {
        get => _totalMilkAmount;
        set => SetProperty(ref _totalMilkAmount, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private async Task LoadCustomersAsync()
    {
        var customers = await _customerService.GetActiveCustomersAsync();
        Customers = new ObservableCollection<CustomerDto>(customers);
    }

    private bool CanSave()
    {
        return SelectedCustomer != null && TotalMilkAmount > 0;
    }

    private async Task SaveAsync()
    {
        try
        {
            IsLoading = true;
            
            await _milkCycleService.CreateCycleAsync(
                SelectedCustomer!.CustomerId,
                StartDate,
                EndDate,
                TotalMilkAmount,
                "Admin"
            );

            _dialogService.ShowMessage("Milk Cycle created successfully.");
            _navigationService.NavigateTo<MilkCycleListViewModel>();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Error: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Cancel()
    {
        _navigationService.NavigateTo<MilkCycleListViewModel>();
    }
}
