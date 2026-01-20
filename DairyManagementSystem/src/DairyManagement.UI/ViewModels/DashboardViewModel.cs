using DairyManagement.Application.Interfaces;

namespace DairyManagement.UI.ViewModels;

/// <summary>
/// Dashboard ViewModel showing key metrics and alerts
/// </summary>
public class DashboardViewModel : BaseViewModel
{
    private readonly ICustomerService _customerService;
    private readonly IProductService _productService;
    private readonly IStockService _stockService;
    private readonly ISettlementService _settlementService;

    private int _totalActiveCustomers;
    private int _totalActiveProducts;
    private int _pendingSettlements;
    private int _lowStockProducts;

    public DashboardViewModel(
        ICustomerService customerService,
        IProductService productService,
        IStockService stockService,
        ISettlementService settlementService)
    {
        _customerService = customerService;
        _productService = productService;
        _stockService = stockService;
        _settlementService = settlementService;

        // Load dashboard data
        _ = LoadDashboardDataAsync();
    }

    public int TotalActiveCustomers
    {
        get => _totalActiveCustomers;
        set => SetProperty(ref _totalActiveCustomers, value);
    }

    public int TotalActiveProducts
    {
        get => _totalActiveProducts;
        set => SetProperty(ref _totalActiveProducts, value);
    }

    public int PendingSettlements
    {
        get => _pendingSettlements;
        set => SetProperty(ref _pendingSettlements, value);
    }

    public int LowStockProducts
    {
        get => _lowStockProducts;
        set => SetProperty(ref _lowStockProducts, value);
    }

    private async Task LoadDashboardDataAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            // Load metrics
            var customers = await _customerService.GetActiveCustomersAsync();
            TotalActiveCustomers = customers.Count();

            var products = await _productService.GetActiveProductsAsync();
            TotalActiveProducts = products.Count();

            var unpaidSettlements = await _settlementService.GetUnpaidSettlementsAsync();
            PendingSettlements = unpaidSettlements.Count();

            var lowStockProducts = await _stockService.GetProductsNeedingReorderAsync();
            LowStockProducts = lowStockProducts.Count();
        }
        catch (Exception ex)
        {
            SetError($"Failed to load dashboard data: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
