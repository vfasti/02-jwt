using JwtAuthenticationTemplateNet9.DTO;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace JwtAuthenticationTemplateNet9.Services;

public class JWTService : ITokenService             // Schritt 3c
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly IConfiguration _configuration;

    public JWTService(UserManager<IdentityUser> userManager, IConfiguration configuration)
    {
        _userManager = userManager;
        _configuration = configuration;
    }

    public async Task<TokenDto> CreateTokenAsync(IdentityUser user)
    {
        IEnumerable<string> userRoles = await _userManager.GetRolesAsync(user);

        List<Claim> authClaims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),        // subject - unique identifier for the user
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()), // JWT ID - unique identifier for the token
            new Claim(JwtRegisteredClaimNames.Name, user.UserName!) // add more claims if needed: e.g. name, email, ...
        };

        foreach (string userRole in userRoles)                      // add user roles as claims
        {
            authClaims.Add(new Claim(ClaimTypes.Role, userRole));
        }

        SymmetricSecurityKey authSigningKey = new SymmetricSecurityKey(
            Encoding.ASCII.GetBytes(_configuration.GetSection("JwtSettings")["Secret"]!));

        // create token with expiring date, claims and signing credentials
        JwtSecurityToken token = new JwtSecurityToken(  
            expires: DateTime.Now.AddMinutes(15),       // token valid for 15 minutes
            claims: authClaims,                         // user claims
            signingCredentials: new SigningCredentials( // signing the token with secret key
                  authSigningKey, SecurityAlgorithms.HmacSha256));

        return new TokenDto()
        {
            Token = new JwtSecurityTokenHandler().WriteToken(token),
            Expiration = token.ValidTo
        };
    }
}
