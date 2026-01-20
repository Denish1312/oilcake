using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class SettlementViewModel : BaseViewModel
{
    private readonly ISettlementService _settlementService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;
    private readonly IReceiptService _receiptService;
    private readonly IPrintService _printService;

    private int _milkCycleId;
    private SettlementDto? _preview;
    private string _paymentMode = "Cash";

    public SettlementViewModel(
        ISettlementService settlementService,
        IDialogService dialogService,
        INavigationService navigationService,
        IReceiptService receiptService,
        IPrintService printService)
    {
        _settlementService = settlementService;
        _dialogService = dialogService;
        _navigationService = navigationService;
        _receiptService = receiptService;
        _printService = printService;

        PreviewCommand = new AsyncRelayCommand(PreviewAsync);
        CreateSettlementCommand = new AsyncRelayCommand(CreateSettlementAsync, () => Preview != null);
        PrintReceiptCommand = new AsyncRelayCommand(PrintReceiptAsync, () => Preview != null);
        CancelCommand = new RelayCommand(Cancel);
    }

    public ICommand PreviewCommand { get; }
    public ICommand CreateSettlementCommand { get; }
    public ICommand PrintReceiptCommand { get; }
    public ICommand CancelCommand { get; }
    
    public SettlementDto? Preview
    {
        get => _preview;
        set
        {
            if (SetProperty(ref _preview, value))
            {
                ((AsyncRelayCommand)CreateSettlementCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)PrintReceiptCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string PaymentMode
    {
        get => _paymentMode;
        set => SetProperty(ref _paymentMode, value);
    }

    public List<string> PaymentModes => new() { "Cash", "UPI", "BankTransfer", "Cheque" };

    // This would be called when navigating to this VM with a cycle ID
    public void SetMilkCycle(int milkCycleId)
    {
        _milkCycleId = milkCycleId;
        _ = PreviewAsync();
    }

    private async Task PreviewAsync()
    {
        try
        {
            IsLoading = true;
            Preview = await _settlementService.PreviewSettlementAsync(_milkCycleId);
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Preview failed: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task CreateSettlementAsync()
    {
        try
        {
            IsLoading = true;
            
            var request = new CreateSettlementDto
            {
                CycleId = _milkCycleId,
                PaymentMode = PaymentMode.ToUpperInvariant()
            };

            var result = await _settlementService.CreateSettlementAsync(request, "Admin");
            Preview = result; // Update with saved result (includes ID)
            _dialogService.ShowMessage("Settlement created successfully!");
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to create settlement: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task PrintReceiptAsync()
    {
        if (Preview == null) return;

        try
        {
            IsLoading = true;
            var receiptText = _receiptService.GenerateSettlementReceipt(Preview);
            await _printService.PrintRawTextAsync(receiptText);
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Printing failed: {ex.Message}");
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
