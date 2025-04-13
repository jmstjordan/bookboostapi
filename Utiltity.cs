
using BookBoostApi.Models;

public static class Utility
{
    public static string GetUserId(this HttpContext context)
    {
        return context.User.Claims.FirstOrDefault(c => c.Type == CustomClaimTypes.UserId)?.Value;
    }

    public static string GetRequestHost(this HttpContext context)
    {
        return context.Request.Headers["Origin"].FirstOrDefault();
    }

    public static string GetUsername(this string email)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains("@"))
            return string.Empty; // Handle invalid email cases

        return email.Split('@')[0]; // Extract username before '@'
    }

    public static string GenerateShortGuid(this Guid guid)
    {
        return Convert.ToBase64String(guid.ToByteArray())
            .Replace("/", "_")
            .Replace("+", "-")
            .Substring(0, 22); // remove trailing ==
    }
}