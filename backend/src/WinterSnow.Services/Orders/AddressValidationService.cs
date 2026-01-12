namespace WinterSnow.Services.Orders;

/// <summary>
/// Placeholder for real address validation (India PIN validation, etc.).
/// </summary>
public class AddressValidationService : IAddressValidationService
{
    public Task<AddressValidationResult> ValidateAsync(ShippingAddressInput input, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(input.PostalCode) || input.PostalCode.Length < 5)
        {
            return Task.FromResult(new AddressValidationResult
            {
                IsValid = false,
                Message = "Invalid postal code."
            });
        }

        var normalized = new ShippingAddressInput
        {
            FullName = input.FullName.Trim(),
            Line1 = input.Line1.Trim(),
            Line2 = input.Line2?.Trim(),
            City = input.City.Trim(),
            State = input.State.Trim(),
            PostalCode = input.PostalCode.Trim(),
            CountryCode = string.IsNullOrWhiteSpace(input.CountryCode) ? "IN" : input.CountryCode.Trim().ToUpperInvariant(),
            Phone = input.Phone?.Trim()
        };

        return Task.FromResult(new AddressValidationResult
        {
            IsValid = true,
            Normalized = normalized
        });
    }
}

