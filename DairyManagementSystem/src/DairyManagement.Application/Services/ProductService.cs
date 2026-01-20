using DairyManagement.Application.DTOs;
using DairyManagement.Application.Interfaces;
using DairyManagement.Domain.Entities;
using DairyManagement.Infrastructure.Repositories;

namespace DairyManagement.Application.Services;

/// <summary>
/// Product service implementation
/// </summary>
public class ProductService : IProductService
{
    private readonly IUnitOfWork _unitOfWork;

    public ProductService(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    public async Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetAllAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<IEnumerable<ProductDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default)
    {
        var products = await _unitOfWork.Products.GetActiveProductsAsync(cancellationToken);
        return products.Select(MapToDto);
    }

    public async Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto?> GetProductByCodeAsync(string productCode, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByCodeAsync(productCode, cancellationToken);
        return product == null ? null : MapToDto(product);
    }

    public async Task<ProductDto> CreateProductAsync(CreateProductDto dto, string username, CancellationToken cancellationToken = default)
    {
        // Check if code already exists
        if (await _unitOfWork.Products.CodeExistsAsync(dto.ProductCode, cancellationToken))
            throw new InvalidOperationException($"Product code '{dto.ProductCode}' already exists");

        // Create product
        var product = Product.Create(
            dto.ProductCode,
            dto.ProductName,
            dto.UnitOfMeasure,
            dto.UnitPrice,
            dto.ReorderLevel,
            dto.Description
        );
        product.SetCreatedBy(username);

        // Save
        await _unitOfWork.Products.AddAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        return MapToDto(product);
    }

    public async Task UpdateProductAsync(int productId, UpdateProductDto dto, string username, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException($"Product {productId} not found");

        // Update
        product.Update(
            dto.ProductName,
            dto.UnitPrice,
            dto.ReorderLevel,
            dto.Description
        );
        product.SetUpdatedBy(username);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task ActivateProductAsync(int productId, string username, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException($"Product {productId} not found");

        product.Activate();
        product.SetUpdatedBy(username);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task DeactivateProductAsync(int productId, string username, CancellationToken cancellationToken = default)
    {
        var product = await _unitOfWork.Products.GetByIdAsync(productId, cancellationToken);
        if (product == null)
            throw new InvalidOperationException($"Product {productId} not found");

        product.Deactivate();
        product.SetUpdatedBy(username);

        await _unitOfWork.Products.UpdateAsync(product, cancellationToken);
        await _unitOfWork.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> ProductCodeExistsAsync(string productCode, CancellationToken cancellationToken = default)
    {
        return await _unitOfWork.Products.CodeExistsAsync(productCode, cancellationToken);
    }

    private static ProductDto MapToDto(Product product)
    {
        return new ProductDto
        {
            ProductId = product.ProductId,
            ProductCode = product.ProductCode,
            ProductName = product.ProductName,
            Description = product.Description,
            UnitOfMeasure = product.UnitOfMeasure,
            UnitPrice = product.UnitPrice.Amount,
            CurrentStock = product.CurrentStock.Value,
            ReorderLevel = product.ReorderLevel.Value,
            StockStatus = product.GetStockStatus().ToString(),
            IsActive = product.IsActive
        };
    }
}
