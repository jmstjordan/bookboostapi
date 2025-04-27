using System.Diagnostics;
using System.Text;

public class ActivityTaggingMiddleware
{
    private readonly RequestDelegate _next;

    public ActivityTaggingMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        var activity = Activity.Current;
        if (activity != null && context.User.Identity?.IsAuthenticated == true)
        {
            var userId = context.GetUserId();
            activity.SetTag("UserId", userId);
        }
        await _next(context);
    }
}
