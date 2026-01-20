namespace DairyManagement.Domain.Enums;

/// <summary>
/// Stock status based on reorder level
/// </summary>
public enum StockStatus
{
    /// <summary>
    /// Stock is critically low (at or below reorder level)
    /// </summary>
    LowStock = 1,
    
    /// <summary>
    /// Stock is medium (between reorder level and 1.5x reorder level)
    /// </summary>
    MediumStock = 2,
    
    /// <summary>
    /// Stock is good (above 1.5x reorder level)
    /// </summary>
    GoodStock = 3,
    
    /// <summary>
    /// Stock is out (zero or negative)
    /// </summary>
    OutOfStock = 4
}
