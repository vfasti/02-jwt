using JwtAuthenticationTemplateNet9.DTO;
using JwtAuthenticationTemplateNet9.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace JwtAuthenticationTemplateNet9.Controllers;

[AllowAnonymous]                // Schritt 2b
[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly UserManager<IdentityUser> _userManager;
    private readonly ITokenService _tokenService;          // Schritt 4b

    public AuthController(UserManager<IdentityUser> userManager, ITokenService tokenService)
    {
        _userManager = userManager;
        _tokenService = tokenService;
    }

    [HttpPost]
    [Route("register")]
    public async Task<IActionResult> RegisterAsync(RegisterDto registerData)    // Schritt 2b
    {
        if (registerData == null)   // check user data
            return BadRequest();

        try
        {
            IdentityUser? user = await _userManager.FindByNameAsync(registerData.Username);

            if (user != null)   // check if username already exists
                return Conflict("Username already exists!");

            IdentityUser newUser = new IdentityUser()   // create new object for new user
            {
                Email = registerData.Email,
                SecurityStamp = Guid.NewGuid().ToString(),
                UserName = registerData.Username,
            };

            // try to create new user, if password rules are met and username and email is unique
            IdentityResult resultCreate = await _userManager.CreateAsync(newUser, registerData.Password);

            if (!resultCreate.Succeeded)
                return BadRequest("User creation failed! Please check user details and try again.");

            if (_userManager.SupportsUserRole)  // checks if user manager supports roles
            {
                IdentityResult resultRole = await _userManager.AddToRoleAsync(newUser, "user");
                if (!resultRole.Succeeded)      // add role "user" to newly created user
                    StatusCode(StatusCodes.Status500InternalServerError, "User was created, but role assignment failed! Please check role configuration.");
            }
            else
            {
                return Ok("User created successfully, but roles are not supported!");
            }

            return Ok("User created successfully!");
        }
        catch (DbUpdateException)
        {
            return BadRequest("User creation failed! Please check user and password requirements and try again.");
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }


    // POST: api/auth/login
    [HttpPost]
    [Route("login")]
    public async Task<IActionResult> LoginAsync(LoginDto loginData)      // Schritt 4b
    {
        if (loginData == null)
            return BadRequest();

        try
        {   // check if user exists and password is correct
            IdentityUser? user = await _userManager.FindByNameAsync(loginData.Username);

            if (user != null && await _userManager.CheckPasswordAsync(user, loginData.Password))
                return Ok(await _tokenService.CreateTokenAsync(user!)); // create token and return it

            return Unauthorized();  // username or password is incorrect - do not reveal which one
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, ex.Message);
        }
    }
}