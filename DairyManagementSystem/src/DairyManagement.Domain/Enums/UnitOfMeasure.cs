namespace DairyManagement.Domain.Enums;

/// <summary>
/// Unit of measure for products
/// </summary>
public enum UnitOfMeasure
{
    /// <summary>
    /// Kilogram
    /// </summary>
    Kilogram = 1,
    
    /// <summary>
    /// Bag (typically 25-50 kg)
    /// </summary>
    Bag = 2,
    
    /// <summary>
    /// Piece/Unit
    /// </summary>
    Piece = 3,
    
    /// <summary>
    /// Liter
    /// </summary>
    Liter = 4,
    
    /// <summary>
    /// Quintal (100 kg)
    /// </summary>
    Quintal = 5
}
