using BeverageGalleryBotWebApi.Service;

namespace BeverageGalleryBotWebApi.Component;

public class RequestRateLimitingMiddleware(ILogger<RequestRateLimitingMiddleware> logger, RequestDelegate next, RedisRateLimiter rateLimiter)
{

    public async Task Invoke(HttpContext context)
    {
        var ip = this.GetClientIp(context);
        var path = context.Request.Path.ToString();
        if (await rateLimiter.IsLimitedAsync(ip))
        {
            logger.LogWarning("Rate limit exceeded: IP={IP}, Path={Path}", ip, path);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new
            {
                Status = "error",
                Message = "Too many requests. Please try again later."
            });
            return;
        }
        
        await next(context);
    }
    
    private string GetClientIp(HttpContext context)
    {
        // Cloudflare header
        if (context.Request.Headers.TryGetValue("CF-Connecting-IP", out var cfIp))
        {
            return cfIp.FirstOrDefault() ?? "unknown";
        }

        // X-Forwarded-For
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim() ?? "unknown";
        }

        // localhost
        return context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
    }
}