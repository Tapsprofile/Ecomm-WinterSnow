namespace WinterSnow.Core.Domain.Orders;

public enum OrderStatus
{
    Pending = 10,
    AwaitingPickup = 20,
    Shipped = 30,
    Completed = 40,
    Cancelled = 50,
    Refunded = 60
}

