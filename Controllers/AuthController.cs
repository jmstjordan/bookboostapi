using BookBoostApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{

    IUserService _userService;

    IAuthService _authService;

    public AuthController(IUserService userService, IAuthService authService)
    {
        _userService = userService;
        _authService = authService;
    }

    [HttpPost("Signup")]
    public async Task<IActionResult> Signup([FromBody] LoginRequest request)
    {
        if (await _userService.GetUserbyEmail(request.Email) != null)
        {
            return BadRequest("User with this Email already exists.");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = hashedPassword
        };
        await _userService.CreatUser(user);
        var token = _authService.GenerateJwtToken(user);
        return Ok(new { token });
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.GetUserbyEmail(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }

        var token = _authService.GenerateJwtToken(user);
        return Ok(new { token });
    }
}