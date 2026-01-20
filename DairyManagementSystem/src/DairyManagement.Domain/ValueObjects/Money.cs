namespace DairyManagement.Domain.ValueObjects;

/// <summary>
/// Value object representing money in Indian Rupees
/// Ensures immutability and validation
/// </summary>
public sealed class Money : IEquatable<Money>, IComparable<Money>
{
    /// <summary>
    /// Amount in Indian Rupees
    /// </summary>
    public decimal Amount { get; }

    /// <summary>
    /// Currency code (always INR for this system)
    /// </summary>
    public string Currency { get; }

    private Money(decimal amount, string currency = "INR")
    {
        Amount = amount;
        Currency = currency;
    }

    /// <summary>
    /// Creates a Money instance with the specified amount
    /// </summary>
    /// <param name="amount">Amount in rupees</param>
    /// <returns>Money instance</returns>
    /// <exception cref="ArgumentException">Thrown when amount is negative</exception>
    public static Money FromAmount(decimal amount)
    {
        if (amount < 0)
            throw new ArgumentException("Amount cannot be negative", nameof(amount));

        return new Money(amount);
    }

    /// <summary>
    /// Creates a Money instance allowing negative amounts (for debts/balances)
    /// </summary>
    /// <param name="amount">Amount in rupees (can be negative)</param>
    /// <returns>Money instance</returns>
    public static Money FromBalance(decimal amount)
    {
        return new Money(amount);
    }

    /// <summary>
    /// Zero money
    /// </summary>
    public static Money Zero => new(0);

    /// <summary>
    /// Adds two money amounts
    /// </summary>
    public static Money operator +(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot add money with different currencies");

        return new Money(left.Amount + right.Amount, left.Currency);
    }

    /// <summary>
    /// Subtracts two money amounts
    /// </summary>
    public static Money operator -(Money left, Money right)
    {
        if (left.Currency != right.Currency)
            throw new InvalidOperationException("Cannot subtract money with different currencies");

        return new Money(left.Amount - right.Amount, left.Currency);
    }

    /// <summary>
    /// Multiplies money by a quantity
    /// </summary>
    public static Money operator *(Money money, decimal multiplier)
    {
        return new Money(money.Amount * multiplier, money.Currency);
    }

    /// <summary>
    /// Checks if amount is positive
    /// </summary>
    public bool IsPositive => Amount > 0;

    /// <summary>
    /// Checks if amount is negative
    /// </summary>
    public bool IsNegative => Amount < 0;

    /// <summary>
    /// Checks if amount is zero
    /// </summary>
    public bool IsZero => Amount == 0;

    /// <summary>
    /// Formats money as Indian Rupee string
    /// </summary>
    public override string ToString()
    {
        return $"₹{Amount:N2}";
    }

    /// <summary>
    /// Formats money with explicit sign
    /// </summary>
    public string ToStringWithSign()
    {
        var sign = Amount >= 0 ? "+" : "";
        return $"{sign}₹{Amount:N2}";
    }

    #region Equality and Comparison

    public bool Equals(Money? other)
    {
        if (other is null) return false;
        return Amount == other.Amount && Currency == other.Currency;
    }

    public override bool Equals(object? obj)
    {
        return obj is Money other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Amount, Currency);
    }

    public int CompareTo(Money? other)
    {
        if (other is null) return 1;
        if (Currency != other.Currency)
            throw new InvalidOperationException("Cannot compare money with different currencies");
        
        return Amount.CompareTo(other.Amount);
    }

    public static bool operator ==(Money? left, Money? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(Money? left, Money? right)
    {
        return !(left == right);
    }

    public static bool operator <(Money left, Money right)
    {
        return left.CompareTo(right) < 0;
    }

    public static bool operator >(Money left, Money right)
    {
        return left.CompareTo(right) > 0;
    }

    public static bool operator <=(Money left, Money right)
    {
        return left.CompareTo(right) <= 0;
    }

    public static bool operator >=(Money left, Money right)
    {
        return left.CompareTo(right) >= 0;
    }

    #endregion
}
