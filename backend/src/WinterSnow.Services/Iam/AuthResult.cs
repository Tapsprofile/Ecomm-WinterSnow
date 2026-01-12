namespace WinterSnow.Services.Iam;

public class AuthResult
{
    public required string AccessToken { get; set; }
    public required string Role { get; set; }
    public int? VendorId { get; set; }
}

