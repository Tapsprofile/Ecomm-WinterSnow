namespace WinterSnow.Services.Orders;

public interface IAddressValidationService
{
    Task<AddressValidationResult> ValidateAsync(ShippingAddressInput input, CancellationToken ct = default);
}

