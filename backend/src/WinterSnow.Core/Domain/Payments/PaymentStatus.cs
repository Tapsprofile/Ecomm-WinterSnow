namespace WinterSnow.Core.Domain.Payments;

public enum PaymentStatus
{
    Pending = 10,
    Authorized = 20,
    Paid = 30,
    Failed = 40,
    Refunded = 50
}

