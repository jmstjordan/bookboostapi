
using BookBoostApi.Models;

public static class Utility
{
    public static string GetRole(this HttpContext context)
    {
        var url = context.Request.Headers["Origin"].FirstOrDefault();
        return url;
    }

    public static string GetUsername(this string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            return string.Empty; // Handle invalid email cases

        return email.Split('@')[0]; // Extract username before '@'
    }
}