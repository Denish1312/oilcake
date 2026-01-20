namespace DairyManagement.Application.DTOs;

/// <summary>
/// DTO for Product entity
/// </summary>
public class ProductDto
{
    public int ProductId { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal CurrentStock { get; set; }
    public decimal ReorderLevel { get; set; }
    public string StockStatus { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}

/// <summary>
/// DTO for creating a new product
/// </summary>
public class CreateProductDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string UnitOfMeasure { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
    public decimal ReorderLevel { get; set; }
}

/// <summary>
/// DTO for updating a product
/// </summary>
public class UpdateProductDto
{
    public string ProductName { get; set; } = string.Empty;
    public string? Description { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal ReorderLevel { get; set; }
}
