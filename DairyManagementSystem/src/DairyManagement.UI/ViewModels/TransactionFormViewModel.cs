using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

public class TransactionFormViewModel : BaseViewModel
{
    private readonly IStockService _stockService;
    private readonly ISettlementService _settlementService;
    private readonly IProductService _productService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private int _customerId;
    private int _milkCycleId;
    private string _customerName = string.Empty;
    private bool _isProductSale = true;
    private ObservableCollection<ProductDto> _products = new();
    private ProductDto? _selectedProduct;
    private decimal _quantity = 1;
    private decimal _amount;
    private DateTime _transactionDate = DateTime.Now;

    public TransactionFormViewModel(
        IStockService stockService,
        ISettlementService settlementService,
        IProductService productService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _stockService = stockService;
        _settlementService = settlementService;
        _productService = productService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        SaveCommand = new AsyncRelayCommand(SaveAsync, CanSave);
        CancelCommand = new RelayCommand(Cancel);

        _ = LoadProductsAsync();
    }

    public void SetContext(int customerId, string customerName, int milkCycleId)
    {
        _customerId = customerId;
        CustomerName = customerName;
        _milkCycleId = milkCycleId;
    }

    public string CustomerName
    {
        get => _customerName;
        set => SetProperty(ref _customerName, value);
    }

    public bool IsProductSale
    {
        get => _isProductSale;
        set
        {
            if (SetProperty(ref _isProductSale, value))
            {
                OnPropertyChanged(nameof(IsAdvancePayment));
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool IsAdvancePayment
    {
        get => !_isProductSale;
        set => IsProductSale = !value;
    }

    public ObservableCollection<ProductDto> Products
    {
        get => _products;
        set => SetProperty(ref _products, value);
    }

    public ProductDto? SelectedProduct
    {
        get => _selectedProduct;
        set
        {
            if (SetProperty(ref _selectedProduct, value))
            {
                if (_selectedProduct != null) Amount = _selectedProduct.UnitPrice;
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
            {
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public decimal Amount
    {
        get => _amount;
        set
        {
            if (SetProperty(ref _amount, value))
            {
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public DateTime TransactionDate
    {
        get => _transactionDate;
        set => SetProperty(ref _transactionDate, value);
    }

    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }

    private async Task LoadProductsAsync()
    {
        var products = await _productService.GetActiveProductsAsync();
        Products = new ObservableCollection<ProductDto>(products);
    }

    private bool CanSave()
    {
        if (IsProductSale)
            return SelectedProduct != null && Quantity > 0 && Amount > 0;
        else
            return Amount > 0;
    }

    private async Task SaveAsync()
    {
        try
        {
            IsLoading = true;
            if (IsProductSale)
            {
                await _stockService.RecordSaleAsync(
                    _customerId,
                    SelectedProduct!.ProductId,
                    _milkCycleId,
                    Quantity,
                    Amount,
                    "Admin"
                );
                _dialogService.ShowMessage("Product sale recorded successfully.");
            }
            else
            {
                // Note: I don't have an IAdvancePaymentService yet.
                // Let's add the functionality to ISettlementService or a new service.
                // For now, I'll add record advance to ISettlementService since it's related.
                await _settlementService.RecordAdvancePaymentAsync(_customerId, _milkCycleId, Amount, "Admin");
                _dialogService.ShowMessage("Advance payment recorded successfully.");
            }

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
