using JwtAuthenticationTemplateNet9.DTO;
using Microsoft.AspNetCore.Identity;

namespace JwtAuthenticationTemplateNet9.Services;

public interface ITokenService  // Schritt 3b   
{
    public Task<TokenDto> CreateTokenAsync(IdentityUser user);
}
