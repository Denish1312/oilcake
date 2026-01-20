namespace DairyManagement.Domain.ValueObjects;

/// <summary>
/// Value object representing a quantity with unit of measure
/// Ensures immutability and validation
/// </summary>
public sealed class Quantity : IEquatable<Quantity>, IComparable<Quantity>
{
    /// <summary>
    /// Numeric value of the quantity
    /// </summary>
    public decimal Value { get; }

    /// <summary>
    /// Unit of measure (KG, BAG, etc.)
    /// </summary>
    public string Unit { get; }

    private Quantity(decimal value, string unit)
    {
        Value = value;
        Unit = unit;
    }

    /// <summary>
    /// Creates a Quantity instance
    /// </summary>
    /// <param name="value">Quantity value</param>
    /// <param name="unit">Unit of measure</param>
    /// <returns>Quantity instance</returns>
    /// <exception cref="ArgumentException">Thrown when value is negative or unit is empty</exception>
    public static Quantity Create(decimal value, string unit)
    {
        if (value < 0)
            throw new ArgumentException("Quantity cannot be negative", nameof(value));

        if (string.IsNullOrWhiteSpace(unit))
            throw new ArgumentException("Unit cannot be empty", nameof(unit));

        return new Quantity(value, unit.ToUpperInvariant());
    }

    /// <summary>
    /// Zero quantity
    /// </summary>
    public static Quantity Zero(string unit) => new(0, unit.ToUpperInvariant());

    /// <summary>
    /// Adds two quantities (must have same unit)
    /// </summary>
    public static Quantity operator +(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new InvalidOperationException($"Cannot add quantities with different units: {left.Unit} and {right.Unit}");

        return new Quantity(left.Value + right.Value, left.Unit);
    }

    /// <summary>
    /// Subtracts two quantities (must have same unit)
    /// </summary>
    public static Quantity operator -(Quantity left, Quantity right)
    {
        if (left.Unit != right.Unit)
            throw new InvalidOperationException($"Cannot subtract quantities with different units: {left.Unit} and {right.Unit}");

        var result = left.Value - right.Value;
        if (result < 0)
            throw new InvalidOperationException("Resulting quantity cannot be negative");

        return new Quantity(result, left.Unit);
    }

    /// <summary>
    /// Multiplies quantity by a factor
    /// </summary>
    public static Quantity operator *(Quantity quantity, decimal multiplier)
    {
        if (multiplier < 0)
            throw new ArgumentException("Multiplier cannot be negative", nameof(multiplier));

        return new Quantity(quantity.Value * multiplier, quantity.Unit);
    }

    /// <summary>
    /// Checks if quantity is zero
    /// </summary>
    public bool IsZero => Value == 0;

    /// <summary>
    /// Checks if quantity is positive
    /// </summary>
    public bool IsPositive => Value > 0;

    /// <summary>
    /// Checks if this quantity is sufficient for the required quantity
    /// </summary>
    public bool IsSufficientFor(Quantity required)
    {
        if (Unit != required.Unit)
            throw new InvalidOperationException($"Cannot compare quantities with different units: {Unit} and {required.Unit}");

        return Value >= required.Value;
    }

    /// <summary>
    /// Formats quantity as string
    /// </summary>
    public override string ToString()
    {
        return $"{Value:N2} {Unit}";
    }

    #region Equality and Comparison

    public bool Equals(Quantity? other)
    {
        if (other is null) return false;
        return Value == other.Value && Unit == other.Unit;
    }

    public override bool Equals(object? obj)
    {
        return obj is Quantity other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Value, Unit);
    }

    public int CompareTo(Quantity? other)
    {
        if (other is null) return 1;
        if (Unit != other.Unit)
            throw new InvalidOperationException($"Cannot compare quantities with different units: {Unit} and {other.Unit}");

        return Value.CompareTo(other.Value);
    }

    public static bool operator ==(Quantity? left, Quantity? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Quantity? left, Quantity? right)
    {
        return !(left == right);
    }

    public static bool operator <(Quantity left, Quantity right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(Quantity left, Quantity right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(Quantity left, Quantity right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(Quantity left, Quantity right)
    {
        return left.CompareTo(right) >= 0;
    }

    #endregion
}
