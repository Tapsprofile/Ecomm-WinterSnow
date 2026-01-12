namespace WinterSnow.Core.Domain.Returns;

public enum ReturnStatus
{
    Requested = 10,
    VendorApproved = 20,
    VendorRejected = 30,
    LabelGenerated = 40,
    PickedUp = 50,
    Received = 60,
    Refunded = 70,
    Closed = 80
}

