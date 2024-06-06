namespace Zlagoda.Server.Models;

public class JwtOptions
{
    public string Issuer { get; set; }
    public string Key { get; set; }
    public int ClockSkewMinutes { get; set; }
    public int ExpiresMinutes { get; set; }
    public int RefreshExpiresDays { get; set; }
}
