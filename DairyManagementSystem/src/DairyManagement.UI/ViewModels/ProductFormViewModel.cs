using System.Windows.Input;
using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.UI.Commands;
using DairyManagement.UI.Services;

namespace DairyManagement.UI.ViewModels;

/// <summary>
/// ViewModel for Product form (Create/Edit)
/// </summary>
public class ProductFormViewModel : BaseViewModel
{
    private readonly IProductService _productService;
    private readonly IDialogService _dialogService;
    private readonly INavigationService _navigationService;

    private int? _productId;
    private string _productCode = string.Empty;
    private string _productName = string.Empty;
    private string _description = string.Empty;
    private string _unitOfMeasure = "KG";
    private decimal _unitPrice;
    private decimal _reorderLevel;
    private bool _isEditMode;

    public ProductFormViewModel(
        IProductService productService,
        IDialogService dialogService,
        INavigationService navigationService)
    {
        _productService = productService;
        _dialogService = dialogService;
        _navigationService = navigationService;

        // Initialize commands
        SaveCommand = new AsyncRelayCommand(SaveProductAsync, CanSave);
        CancelCommand = new RelayCommand(Cancel);
        GenerateCodeCommand = new AsyncRelayCommand(GenerateProductCodeAsync);

        // Generate code for new products
        if (!IsEditMode)
        {
            _ = GenerateProductCodeAsync();
        }
    }

    public int? ProductId
    {
        get => _productId;
        set => SetProperty(ref _productId, value);
    }

    public string ProductCode
    {
        get => _productCode;
        set => SetProperty(ref _productCode, value);
    }

    public string ProductName
    {
        get => _productName;
        set
        {
            if (SetProperty(ref _productName, value))
            {
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public string Description
    {
        get => _description;
        set => SetProperty(ref _description, value);
    }

    public string UnitOfMeasure
    {
        get => _unitOfMeasure;
        set => SetProperty(ref _unitOfMeasure, value);
    }

    public decimal UnitPrice
    {
        get => _unitPrice;
        set
        {
            if (SetProperty(ref _unitPrice, value))
            {
                ((AsyncRelayCommand)SaveCommand).RaiseCanExecuteChanged();
            }
        }
    }

    public decimal ReorderLevel
    {
        get => _reorderLevel;
        set => SetProperty(ref _reorderLevel, value);
    }

    public bool IsEditMode
    {
        get => _isEditMode;
        set => SetProperty(ref _isEditMode, value);
    }

    public string FormTitle => IsEditMode ? "Edit Product" : "New Product";

    public List<string> AvailableUnits => new() { "KG", "BAG", "PIECE", "LITER", "QUINTAL" };

    // Commands
    public ICommand SaveCommand { get; }
    public ICommand CancelCommand { get; }
    public ICommand GenerateCodeCommand { get; }

    private bool CanSave()
    {
        return !string.IsNullOrWhiteSpace(ProductCode) &&
               !string.IsNullOrWhiteSpace(ProductName) &&
               UnitPrice > 0;
    }

    private async Task GenerateProductCodeAsync()
    {
        try
        {
            var products = await _productService.GetAllProductsAsync();
            var maxCode = products
                .Select(p => p.ProductCode)
                .Where(code => code.StartsWith("PROD"))
                .Select(code => int.TryParse(code.Substring(4), out var num) ? num : 0)
                .DefaultIfEmpty(0)
                .Max();

            ProductCode = $"PROD{(maxCode + 1):D3}";
        }
        catch (Exception ex)
        {
            _dialogService.ShowError($"Failed to generate product code: {ex.Message}");
            ProductCode = "PROD001";
        }
    }

    private async Task SaveProductAsync()
    {
        try
        {
            IsLoading = true;
            ClearError();

            if (IsEditMode && ProductId.HasValue)
            {
                var updateDto = new UpdateProductDto
                {
                    ProductName = ProductName,
                    Description = Description,
                    UnitPrice = UnitPrice,
                    ReorderLevel = ReorderLevel
                };

                await _productService.UpdateProductAsync(ProductId.Value, updateDto, "Admin");
                _dialogService.ShowMessage("Product updated successfully");
            }
            else
            {
                var createDto = new CreateProductDto
                {
                    ProductCode = ProductCode,
                    ProductName = ProductName,
                    Description = Description,
                    UnitOfMeasure = UnitOfMeasure,
                    UnitPrice = UnitPrice,
                    ReorderLevel = ReorderLevel
                };

                await _productService.CreateProductAsync(createDto, "Admin");
                _dialogService.ShowMessage("Product created successfully");
            }

            _navigationService.NavigateTo<ProductListViewModel>();
        }
        catch (Exception ex)
        {
            SetError($"Failed to save product: {ex.Message}");
            _dialogService.ShowError($"Failed to save product: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void Cancel()
    {
        _navigationService.NavigateTo<ProductListViewModel>();
    }
}
