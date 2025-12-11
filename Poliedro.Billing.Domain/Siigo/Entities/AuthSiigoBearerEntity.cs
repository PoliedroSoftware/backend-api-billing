namespace Poliedro.Billing.Domain.Siigo.Entities;

public class AuthSiigoBearerEntity
{
    public string AccessToken { get; set; } = string.Empty;
    public string TokenType { get; set; } = string.Empty;
    public int ExpiresIn { get; set; }
}
