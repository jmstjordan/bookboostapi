using BookBoostApi.Interfaces;
using Microsoft.AspNetCore.Mvc;
using BookBoostApi.Models;
using Google.Apis.Auth;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    IUserService _userService;

    IAuthService _authService;

    ITokenService _tokenService;

    IAppEmailService _emailService;

    ISubscriberService _subscriberService;

    public AuthController(IUserService userService, IAuthService authService, ITokenService tokenService, IAppEmailService emailService, ISubscriberService subscriberService)
    {
        _userService = userService;
        _authService = authService;
        _tokenService = tokenService;
        _emailService = emailService;
        _subscriberService = subscriberService;
    }

    [HttpPost("Signup")]
    public async Task<IActionResult> Signup([FromBody] LoginRequest request)
    {
        if (await _userService.GetUserbyEmail(request.Email) != null)
        {
            return Conflict("Account already exists");
        }

        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.Password);
        var user = new User
        {
            Email = request.Email,
            PasswordHash = hashedPassword,
            Role = request.Role,
            Username = request.Email.GetUsername()
        };
        await _userService.CreatUser(user);
        await _emailService.SendUserCreated(user);
        await _subscriberService.AddSubscriber(new Subscriber { Email = request.Email, SubscriberSource = SubscriberSource.BookTokClub });
        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = await _authService.GenerateToken(user, TokenType.Refresh, 168); // 7 days

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
            return Unauthorized("Invalid Credentials");
        }
        if(request.Role != user.Role)
        {
            user.Role = request.Role;
            await _userService.UpdateRole(user.Id, request.Role);
        }

        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = await _authService.GenerateToken(user, TokenType.Refresh, 168);

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken.Token
        });
    }

    [HttpPost("Login/Google")]
    public async Task<IActionResult> GoogleLogin([FromBody] GoogleLoginRequest request)
    {
        var payload = await GoogleJsonWebSignature.ValidateAsync(request.IdToken);

        if (payload == null || !payload.EmailVerified)
        {
            return Unauthorized("Invalid Google token");
        }

        // Check if user exists, or create one
        var user = await _userService.GetUserbyEmail(payload.Email);
        if (user == null)
        {
            user = new User
            {
                Email = payload.Email,
                Role = request.Role,
                Username = payload.Email.GetUsername(),
                Name = payload.Name,
                ProfilePicture = payload.Picture
            };
            await _userService.CreatUser(user);
            await _subscriberService.AddSubscriber(new Subscriber { Email = payload.Email, SubscriberSource = SubscriberSource.BookTokClub });
            await _emailService.SendUserCreated(user);
        }
        else if (user.Name == null)
        {
            await _userService.UpdateUserField(user.Id, "Name", payload.Name);
        }

        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = await _authService.GenerateToken(user, TokenType.Refresh, 168); // 7 days

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken.Token
        });
    }

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh([FromBody] TokenRequest request)
    {
        var storedToken = await _tokenService.GetToken(request.Token);
        if (storedToken == null || storedToken.IsExpired)
        {
            return Unauthorized("Invalid or expired refresh token");
        }
        var user = await _userService.GetUser(storedToken.UserId);
        if (user == null)
        {
            return Unauthorized("User not found");
        }
        await _tokenService.RevokeToken(storedToken);

        var newAccessToken = _authService.GenerateAccessToken(user);
        var newRefreshToken = await _authService.GenerateToken(user, TokenType.Refresh, 168); // 7 days

        return Ok(new
        {
            access_token = newAccessToken,
            refresh_token = newRefreshToken.Token
        });
    }

    [HttpPost("ForgotPassword")]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var user = await _userService.GetUserbyEmail(request.Email);
        if (user == null)
        {
            return BadRequest("Invalid request");
            // don't reveal if a user exists
        }

        var newPasswordToken = await _authService.GenerateToken(user, TokenType.PasswordReset, 1);
        
        var requestHost = HttpContext.GetRequestHost();
        var resetUrl = $"{requestHost}/reset-password?token={Uri.EscapeDataString(newPasswordToken.Token)}&email={Uri.EscapeDataString(user.Email)}";
        await _emailService.SendPasswordReset(user, resetUrl);
        return Ok();
    }

    [HttpPost("ResetPassword")]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var user = await _userService.GetUserbyEmail(request.Email);
        if (user == null)
        {
            return BadRequest("Invalid request");
            // same thing, dont return 404 here
        }
        var passwordToken = await _tokenService.GetToken(request.Token);
        if (passwordToken == null || passwordToken.IsExpired)
        {
            return BadRequest("Invalid request");
            // purposefully ambigiuous here
        }
        var hashedPassword = BCrypt.Net.BCrypt.HashPassword(request.NewPassword);
        await _tokenService.RevokeToken(passwordToken);
        await _userService.UpdatePassword(user.Id, hashedPassword);
        return Ok();
    }
}