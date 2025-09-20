namespace JwtAuthenticationTemplateNet9.DTO;

public class TokenDto       // Schritt 3a
{
    public string Token { get; set; } = null!;
    public DateTime Expiration { get; set; }
}
