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

    [Authorize]
    [HttpGet("{username}")]
    public async Task<IActionResult> GetUser(string username)
    {
        var user = await _userService.GetUser(username);
        if(user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}