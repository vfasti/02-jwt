using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace JwtAuthenticationTemplateNet9.Controllers;

[Route("api/[controller]")]     // 1e - Test Authentication
[ApiController]
public class TestController : ControllerBase
{
    [AllowAnonymous]
    [HttpGet("allowed")]
    public IActionResult OpenRoute()
    {
        return Ok("Open Access");
    }

    [Authorize]
    [HttpGet("secure")]
    public IActionResult SecuredRoute()
    {
        return Ok("Secured Route");
    }

    [Authorize(Roles = "admin")]
    [HttpGet("admin")]
    public IActionResult AdminRoute()
    {
        return Ok("Admin Route");
    }

    [Authorize(Roles = "user")]
    [HttpGet("user")]
    public IActionResult UserRoute()
    {
        return Ok("User Route");
    }

    [Authorize(Roles = "user,admin")]
    [HttpGet("useradmin")]
    public IActionResult UserAdminRoute()
    {
        return Ok("User Admin Route");
    }
}
