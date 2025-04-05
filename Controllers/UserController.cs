using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace BookBoostApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UserController : ControllerBase
{
    private readonly ILogger<AdController> _logger;
    private IAppEmailService _emailService;
    private IUserService _userService;

    public UserController(ILogger<AdController> logger, IAppEmailService emailService, IUserService userService)
    {
        _logger = logger;
        _emailService = emailService;
        _userService = userService;
    }

    [HttpGet("Me")]
    [Authorize]
    public async Task<IActionResult> GetUser()
    {
        var userId = HttpContext.GetUserId();
        var user = await _userService.GetUser(userId);
        if(user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }

    [HttpPost("Preferences")]
    [Authorize]
    public async Task<IActionResult> UpdateUserPreferences([FromBody] Preferences preferences)
    {
        var userId = HttpContext.GetUserId();
        if (userId == null)
        {
            return Unauthorized();
        }
        var updated = await _userService.UpdatePreferences(userId, preferences);
        return Ok(updated);
    }
}