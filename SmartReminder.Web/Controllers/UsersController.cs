using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace SmartReminder.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    [HttpGet("me")]
    public IActionResult GetMe()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var fullName = User.FindFirstValue(ClaimTypes.Name);
        var email = User.FindFirstValue(ClaimTypes.Email);
        var role = User.FindFirstValue(ClaimTypes.Role);

        return Ok(new
        {
            userId,
            fullName,
            email,
            role
        });
    }
}