using DairyManagement.Application.DTOs;

namespace DairyManagement.Application.Interfaces;

/// <summary>
/// Service interface for product operations
/// </summary>
public interface IProductService
{
    /// <summary>
    /// Gets all products
    /// </summary>
    Task<IEnumerable<ProductDto>> GetAllProductsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets active products only
    /// </summary>
    Task<IEnumerable<ProductDto>> GetActiveProductsAsync(CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a product by ID
    /// </summary>
    Task<ProductDto?> GetProductByIdAsync(int productId, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Gets a product by product code
    /// </summary>
    Task<ProductDto?> GetProductByCodeAsync(string productCode, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Creates a new product
    /// </summary>
    Task<ProductDto> CreateProductAsync(CreateProductDto dto, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Updates an existing product
    /// </summary>
    Task UpdateProductAsync(int productId, UpdateProductDto dto, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Activates a product
    /// </summary>
    Task ActivateProductAsync(int productId, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Deactivates a product
    /// </summary>
    Task DeactivateProductAsync(int productId, string username, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Checks if a product code already exists
    /// </summary>
    Task<bool> ProductCodeExistsAsync(string productCode, CancellationToken cancellationToken = default);
}
