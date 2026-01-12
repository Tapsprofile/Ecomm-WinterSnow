namespace WinterSnow.Services.Iam;

public class JwtOptions
{
    public string Issuer { get; set; } = "WinterSnow";
    public string Audience { get; set; } = "WinterSnow";
    public string SigningKey { get; set; } = "CHANGE_ME_DEV_ONLY__PLEASE_OVERRIDE";
    public int AccessTokenMinutes { get; set; } = 120;
}

