namespace WinterSnow.Services.Iam;

public interface IAuthService
{
    Task<AuthResult?> PasswordLoginAsync(string email, string password, CancellationToken ct = default);
}

