using BookBoostApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    IUserService _userService;

    IAuthService _authService;

    IRefreshTokenService _refreshTokenService;

    public AuthController(IUserService userService, IAuthService authService, IRefreshTokenService refreshTokenService)
    {
        _userService = userService;
        _authService = authService;
        _refreshTokenService = refreshTokenService;
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
            PasswordHash = hashedPassword,
            Role = request.Role
        };
        await _userService.CreatUser(user);
        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = await _authService.GenerateRefreshToken(user);

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken.Token
        });
    }


    [HttpPost("Login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userService.GetUserbyEmail(request.Email);

        if (user == null || !BCrypt.Net.BCrypt.Verify(request.Password, user.PasswordHash))
        {
            return Unauthorized("Invalid username or password.");
        }
        if(request.Role != user.Role)
        {
            user.Role = request.Role;
            await _userService.UpdateRole(user.Id, request.Role);
        }

        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = await _authService.GenerateRefreshToken(user);

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken.Token
        });
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] RefreshRequest request)
    {
        var storedToken = await _refreshTokenService.GetToken(request);
        if (storedToken == null || storedToken.IsExpired)
        {
            return Unauthorized("Invalid or expired refresh token");
        }
        var user = await _userService.GetUser(storedToken.UserId);
        if (user == null)
        {
            return Unauthorized("User not found");
        }
        await _refreshTokenService.RevokeToken(storedToken);

        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = await _authService.GenerateRefreshToken(user);

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken.Token
        });
    }
}