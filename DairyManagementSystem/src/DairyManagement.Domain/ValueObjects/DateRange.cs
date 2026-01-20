namespace DairyManagement.Domain.ValueObjects;

/// <summary>
/// Value object representing a date range for milk cycles
/// Ensures immutability and validation
/// </summary>
public sealed class DateRange : IEquatable<DateRange>
{
    /// <summary>
    /// Start date of the range
    /// </summary>
    public DateTime StartDate { get; }

    /// <summary>
    /// End date of the range
    /// </summary>
    public DateTime EndDate { get; }

    private DateRange(DateTime startDate, DateTime endDate)
    {
        StartDate = startDate.Date; // Normalize to date only
        EndDate = endDate.Date;
    }

    /// <summary>
    /// Creates a DateRange instance
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="endDate">End date</param>
    /// <returns>DateRange instance</returns>
    /// <exception cref="ArgumentException">Thrown when end date is before start date</exception>
    public static DateRange Create(DateTime startDate, DateTime endDate)
    {
        if (endDate < startDate)
            throw new ArgumentException("End date cannot be before start date", nameof(endDate));

        return new DateRange(startDate, endDate);
    }

    /// <summary>
    /// Creates a DateRange for a typical 10-day milk cycle
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <returns>DateRange instance spanning 10 days</returns>
    public static DateRange CreateTenDayCycle(DateTime startDate)
    {
        return new DateRange(startDate, startDate.AddDays(9)); // 10 days inclusive
    }

    /// <summary>
    /// Creates a DateRange for a custom number of days
    /// </summary>
    /// <param name="startDate">Start date</param>
    /// <param name="numberOfDays">Number of days in the cycle</param>
    /// <returns>DateRange instance</returns>
    public static DateRange CreateCycle(DateTime startDate, int numberOfDays)
    {
        if (numberOfDays <= 0)
            throw new ArgumentException("Number of days must be positive", nameof(numberOfDays));

        return new DateRange(startDate, startDate.AddDays(numberOfDays - 1));
    }

    /// <summary>
    /// Number of days in the range (inclusive)
    /// </summary>
    public int DurationInDays => (EndDate - StartDate).Days + 1;

    /// <summary>
    /// Checks if a date falls within this range
    /// </summary>
    public bool Contains(DateTime date)
    {
        var normalizedDate = date.Date;
        return normalizedDate >= StartDate && normalizedDate <= EndDate;
    }

    /// <summary>
    /// Checks if this range overlaps with another range
    /// </summary>
    public bool OverlapsWith(DateRange other)
    {
        return StartDate <= other.EndDate && EndDate >= other.StartDate;
    }

    /// <summary>
    /// Checks if the range has ended (end date is in the past)
    /// </summary>
    public bool HasEnded => EndDate < DateTime.Today;

    /// <summary>
    /// Checks if the range is currently active
    /// </summary>
    public bool IsActive => Contains(DateTime.Today);

    /// <summary>
    /// Checks if the range is in the future
    /// </summary>
    public bool IsFuture => StartDate > DateTime.Today;

    /// <summary>
    /// Formats the date range as a string
    /// </summary>
    public override string ToString()
    {
        return $"{StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy}";
    }

    /// <summary>
    /// Formats the date range with duration
    /// </summary>
    public string ToStringWithDuration()
    {
        return $"{StartDate:dd/MM/yyyy} to {EndDate:dd/MM/yyyy} ({DurationInDays} days)";
    }

    #region Equality

    public bool Equals(DateRange? other)
    {
        if (other is null) return false;
        return StartDate == other.StartDate && EndDate == other.EndDate;
    }

    public override bool Equals(object? obj)
    {
        return obj is DateRange other && Equals(other);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(StartDate, EndDate);
    }

    public static bool operator ==(DateRange? left, DateRange? right)
    {
        if (left is null) return right is null;
        return left.Equals(right);
    }

    public static bool operator !=(DateRange? left, DateRange? right)
    {
        return !(left == right);
    }

    #endregion
}
