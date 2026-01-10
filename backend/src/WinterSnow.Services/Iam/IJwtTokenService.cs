using WinterSnow.Core.Domain.Customers;

namespace WinterSnow.Services.Iam;

public interface IJwtTokenService
{
    string CreateAccessToken(AppUser user);
}

