using System.Collections.ObjectModel;
using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

/// <summary>
/// ViewModel for Product list view
/// </summary>
public class ProductListViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private ObservableCollection<ProductDto> _products = new();
    private ProductDto? _selectedProduct;
    private bool _showActiveOnly = true;

    public ProductListViewModel(
        IProductService productService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _productService = productService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        // Initialize commands
        LoadProductsCommand = new AsyncRelayCommand(LoadProductsAsync);
        AddProductCommand = new RelayCommand(AddProduct);
        EditProductCommand = new RelayCommand(EditProduct, () => SelectedProduct != null);
        ActivateProductCommand = new AsyncRelayCommand(ActivateProductAsync, () => SelectedProduct != null && !SelectedProduct.IsActive);
        DeactivateProductCommand = new AsyncRelayCommand(DeactivateProductAsync, () => SelectedProduct != null && SelectedProduct.IsActive);

        // Load products on initialization
        _ = LoadProductsAsync();
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
                ((RelayCommand)EditProductCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)ActivateProductCommand).RaiseCanExecuteChanged();
                ((AsyncRelayCommand)DeactivateProductCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public bool ShowActiveOnly
    {
        get => _showActiveOnly;
        set
        {
            if (SetProperty(ref _showActiveOnly, value))
            {
                _ = LoadProductsAsync();
            }
        }
    }

    // Commands
    public ICommand LoadProductsCommand { get; }
    public ICommand AddProductCommand { get; }
    public ICommand EditProductCommand { get; }
    public ICommand ActivateProductCommand { get; }
    public ICommand DeactivateProductCommand { get; }

    private async Task LoadProductsAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            var products = ShowActiveOnly
                ? await _productService.GetActiveProductsAsync()
                : await _productService.GetAllProductsAsync();

            Products = new ObservableCollection<ProductDto>(products);
        }
        catch (Exception ex)
        {
            SetError($"Failed to load products: {ex.Message}");
            _dialogService.ShowError($"Failed to load products: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void AddProduct()
    {
        _navigationService.NavigateTo<ProductFormViewModel>();
    }

    private void EditProduct()
    {
        if (SelectedProduct == null) return;
        
        // TODO: Pass product ID to form
        _navigationService.NavigateTo<ProductFormViewModel>();
    }

    private async Task ActivateProductAsync()
    {
        if (SelectedProduct == null) return;

        try
        {
            IsLoading = true;
            await _productService.ActivateProductAsync(SelectedProduct.ProductId, "Admin");
            _dialogService.ShowMessage("Product activated successfully");
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to activate product: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private async Task DeactivateProductAsync()
    {
        if (SelectedProduct == null) return;

        var confirm = _dialogService.ShowConfirmation(
            $"Are you sure you want to deactivate product '{SelectedProduct.ProductName}'?",
            "Confirm Deactivation");

        if (!confirm) return;

        try
        {
            IsLoading = true;
            await _productService.DeactivateProductAsync(SelectedProduct.ProductId, "Admin");
            _dialogService.ShowMessage("Product deactivated successfully");
            await LoadProductsAsync();
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to deactivate product: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }
}
