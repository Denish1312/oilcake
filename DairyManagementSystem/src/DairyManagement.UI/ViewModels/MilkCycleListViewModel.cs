using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class MilkCycleListViewModel : BaseViewModel
{
    private readonly IMilkCycleService _milkCycleService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<MilkCycleDto> _milkCycles = new();
    private MilkCycleDto? _selectedMilkCycle;
    private bool _showUnsettledOnly = true;

    public MilkCycleListViewModel(
        IMilkCycleService milkCycleService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _milkCycleService = milkCycleService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        LoadMilkCyclesCommand = new AsyncRelayCommand(LoadMilkCyclesAsync);
        AddMilkCycleCommand = new RelayCommand(AddMilkCycle);
        ViewSettlementCommand = new RelayCommand(ViewSettlement, () => SelectedMilkCycle != null);
        RecordTransactionCommand = new RelayCommand(RecordTransaction, () => SelectedMilkCycle != null && !SelectedMilkCycle.IsSettled);

        _ = LoadMilkCyclesAsync();
    }

    public ObservableCollection<MilkCycleDto> MilkCycles
    {
        get => _milkCycles;
        set => SetProperty(ref _milkCycles, value);
    }

    public MilkCycleDto? SelectedMilkCycle
    {
        get => _selectedMilkCycle;
        set
        {
            if (SetProperty(ref _selectedMilkCycle, value))
            {
                ((RelayCommand)ViewSettlementCommand).RaiseCanExecuteChanged();
                ((RelayCommand)RecordTransactionCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool ShowUnsettledOnly
    {
        get => _showUnsettledOnly;
        set
        {
            if (SetProperty(ref _showUnsettledOnly, value))
            {
                _ = LoadMilkCyclesAsync();
            }
        }
    }

    public ICommand LoadMilkCyclesCommand { get; }
    public ICommand AddMilkCycleCommand { get; }
    public ICommand ViewSettlementCommand { get; }
    public ICommand RecordTransactionCommand { get; }

    private async Task LoadMilkCyclesAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            var cycles = await _milkCycleService.GetAllCyclesAsync(ShowUnsettledOnly);
            MilkCycles = new ObservableCollection<MilkCycleDto>(cycles);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load milk cycles: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddMilkCycle()
    {
        _navigationService.NavigateTo<MilkCycleFormViewModel>();
    }

    private void ViewSettlement()
    {
        if (SelectedMilkCycle == null) return;
        _navigationService.NavigateTo<SettlementViewModel>();
    }

    private void RecordTransaction()
    {
        if (SelectedMilkCycle == null) return;
        
        var vm = _navigationService.NavigateTo<TransactionFormViewModel>();
        vm.SetContext(SelectedMilkCycle.CustomerId, SelectedMilkCycle.CustomerName, SelectedMilkCycle.MilkCycleId);
    }
}

public class MilkCycleDto
{
    public int MilkCycleId { get; set; }
    public int CustomerId { get; set; }
    public string CustomerName { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public decimal TotalMilkAmount { get; set; }
    public bool IsSettled { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string Period => $"{StartDate:dd/MM/yyyy} - {EndDate:dd/MM/yyyy}";
}
