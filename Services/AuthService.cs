using BookBoostApi.Interfaces;
using BookBoostApi.Models;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Security.Cryptography;

namespace BookBoostApi.Services;

public class AuthService : IAuthService
{
    private readonly string _secretKey;

    private readonly string _issuer;

    ITokenService _tokenService;


    public AuthService(IOptions<AuthSettings> authSettings, ITokenService tokenService)
    {
        _secretKey = authSettings.Value.SecretKey;
        _issuer = authSettings.Value.Audience;
        _tokenService = tokenService;
    }

    public string GenerateAccessToken(User user)
    {
        var claims = new List<Claim>
        {
            new Claim(JwtRegisteredClaimNames.Sub, user.Id),
            new Claim(JwtRegisteredClaimNames.Email, user.Email),
            new Claim(JwtRegisteredClaimNames.UniqueName, user.Email.GetUsername()),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Role, user.Role.ToString().ToLower())
        };
        if(user.IsAdmin)
        {
            claims.Add(new Claim(ClaimTypes.Role, "admin"));
        }

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secretKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: _issuer,
            audience: _issuer,
            claims: claims,
            expires: DateTime.UtcNow.AddMinutes(15),
            signingCredentials: creds
        );

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    public async Task<TokenStore> GenerateToken(User user, TokenType tokenType, int expirationHours)
    {
        var token = new TokenStore
        {
            Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64)),
            Expires = DateTime.UtcNow.AddHours(expirationHours),
            UserId = user.Id,
            TokenType = tokenType
        };
        await _tokenService.AddToken(token);
        return token;
    }
}