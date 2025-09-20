namespace JwtAuthenticationTemplateNet9.DTO;

public class RegisterDto                            // Schritt 2a
{
    public string Username { get; set; } = null!;  
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;
}