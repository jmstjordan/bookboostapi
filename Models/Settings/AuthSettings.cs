namespace BookBoostApi.Models;

public class AuthSettings
{
    public string SecretKey { get; set; } = null!;
    
    public string Audience { get; set; } = null!;
}