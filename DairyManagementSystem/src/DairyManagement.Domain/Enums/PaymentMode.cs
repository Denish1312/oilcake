namespace DairyManagement.Domain.Enums;

/// <summary>
/// Payment modes supported by the system
/// </summary>
public enum PaymentMode
{
    /// <summary>
    /// Cash payment
    /// </summary>
    Cash = 1,
    
    /// <summary>
    /// Bank transfer/NEFT/RTGS
    /// </summary>
    BankTransfer = 2,
    
    /// <summary>
    /// Cheque payment
    /// </summary>
    Cheque = 3,
    
    /// <summary>
    /// UPI payment (PhonePe, Google Pay, etc.)
    /// </summary>
    UPI = 4
}
