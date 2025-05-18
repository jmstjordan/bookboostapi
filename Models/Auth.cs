using BookBoostApi.Models;


public class LoginRequest
{
    public required string Email { get; set; }
    
    public required string Password { get; set; }

    public Role Role { get; set; }
}
public class SignUpRequest : LoginRequest
{
    public required string FirstName { get; set; }

    public required string LastName { get; set; }
}

public class GoogleLoginRequest
{
    public string IdToken { get; set; }

    public Role Role { get; set; }
}
