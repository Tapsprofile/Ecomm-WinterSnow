using Microsoft.EntityFrameworkCore;
using WinterSnow.Core.Domain.Customers;
using WinterSnow.Data.Repositories;

namespace WinterSnow.Services.Iam;

public class AuthService : IAuthService
{
    private readonly IRepository<AppUser> _users;
    private readonly IPasswordHasher _passwordHasher;
    private readonly IJwtTokenService _jwtTokenService;

    public AuthService(
        IRepository<AppUser> users,
        IPasswordHasher passwordHasher,
        IJwtTokenService jwtTokenService)
    {
        _users = users;
        _passwordHasher = passwordHasher;
        _jwtTokenService = jwtTokenService;
    }

    public async Task<AuthResult?> PasswordLoginAsync(string email, string password, CancellationToken ct = default)
    {
        var normalized = email.Trim().ToLowerInvariant();
        var user = await _users.Table.FirstOrDefaultAsync(x => x.Email.ToLower() == normalized, ct);
        if (user is null)
            return null;

        if (!_passwordHasher.VerifyHashedPassword(user.PasswordHash, password))
            return null;

        return new AuthResult
        {
            AccessToken = _jwtTokenService.CreateAccessToken(user),
            Role = user.UserType.ToString(),
            VendorId = user.VendorId
        };
    }
}

