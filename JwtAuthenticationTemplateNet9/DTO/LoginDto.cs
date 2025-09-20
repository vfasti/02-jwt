namespace JwtAuthenticationTemplateNet9.DTO;

public class LoginDto                   // Schritt 4a
{
    public string Username { get; set; } = null!;
    public string Password { get; set; } = null!;
}