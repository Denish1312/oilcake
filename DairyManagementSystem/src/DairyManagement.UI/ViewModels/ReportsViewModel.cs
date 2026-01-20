using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class ReportsViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly ISettlementService _settlementService;
    private readonly IDialogService _dialogService;

    private DateTime _dateFrom = DateTime.Now.AddDays(-30);
    private DateTime _dateTo = DateTime.Now;
    private string _selectedReportType = "Settlements";
    private ObservableCollection<object> _reportResults = new();

    public ReportsViewModel(
        ICustomerService customerService,
        IProductService productService,
        ISettlementService settlementService,
        IDialogService dialogService)
    {
        _customerService = customerService;
        _productService = productService;
        _settlementService = settlementService;
        _dialogService = dialogService;

        GenerateReportCommand = new AsyncRelayCommand(GenerateReportAsync);
        ExportCommand = new RelayCommand(ExportReport);
    }

    public DateTime DateFrom
    {
        get => _dateFrom;
        set => SetProperty(ref _dateFrom, value);
    }

    public DateTime DateTo
    {
        get => _dateTo;
        set => SetProperty(ref _dateTo, value);
    }

    public string SelectedReportType
    {
        get => _selectedReportType;
        set => SetProperty(ref _selectedReportType, value);
    }

    public List<string> ReportTypes => new() { "Settlements", "Customers Summary", "Product Stock" };

    public ObservableCollection<object> ReportResults
    {
        get => _reportResults;
        set => SetProperty(ref _reportResults, value);
    }

    public ICommand GenerateReportCommand { get; }
    public ICommand ExportCommand { get; }

    private async Task GenerateReportAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();
            ReportResults.Clear();

            if (SelectedReportType == "Settlements")
            {
                var settlements = await _settlementService.GetSettlementsByDateRangeAsync(DateFrom, DateTo);
                foreach (var item in settlements) ReportResults.Add(item);
            }
            else if (SelectedReportType == "Customers Summary")
            {
                var customers = await _customerService.GetAllCustomersAsync();
                foreach (var item in customers) ReportResults.Add(item);
            }
            else if (SelectedReportType == "Product Stock")
            {
                var products = await _productService.GetAllProductsAsync();
                foreach (var item in products) ReportResults.Add(item);
            }
        }
        catch (Exception ex)
        {
            SetError($"Failed to generate report: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ExportReport()
    {
        _dialogService.ShowMessage("Export to PDF/Excel will be implemented in the next phase.", "Feature Coming Soon");
    }
}
