using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using BookBoostApi.Interfaces;

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

    [HttpPost]
    public async Task<IActionResult> Upsert([FromBody] User user)
    {
        var checkUser = await _userService.GetUser(user.UserId);
        user.Id = checkUser.Id;
        await _userService.UpsertUser(user);
        if(checkUser == null)
        {
            // new user, send email
            await _emailService.SendUserCreated(user);
        }
        return Ok();
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetUser(string userId)
    {
        var user = await _userService.GetUser(userId);
        if(user == null)
        {
            return NotFound();
        }
        return Ok(user);
    }
}