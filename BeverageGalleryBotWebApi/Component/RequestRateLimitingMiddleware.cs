using System.Net;
using BeverageGalleryBotWebApi.Service;

namespace BeverageGalleryBotWebApi.Component;

public class RequestRateLimitingMiddleware(ILogger<RequestRateLimitingMiddleware> logger, RequestDelegate next, RedisRateLimiter rateLimiter)
{
    private static readonly string UnknownIP = "unknown";
    
    public async Task Invoke(HttpContext context)
    {
        // Allow option
        if (HttpMethods.IsOptions(context.Request.Method))
        {
            await next(context);
            return;
        }
        
        var ip = this.GetClientIp(context);
        logger.LogInformation("Client IP resolved: {IP}", ip);
        
        if (UnknownIP.Equals(ip,StringComparison.OrdinalIgnoreCase))
        {
            logger.LogWarning("Failed to resolve client IP. Using fallback: {Remote}", context.Connection.RemoteIpAddress);
        }
        
        var path = context.Request.Path.ToString();
        if (await rateLimiter.IsLimitedAsync(ip))
        {
            logger.LogWarning("Rate limit exceeded: IP={IP}, Path={Path}", ip, path);
            context.Response.StatusCode = StatusCodes.Status429TooManyRequests;
            await context.Response.WriteAsJsonAsync(new
            {
                Code = HttpStatusCode.TooManyRequests,
                Status = "error",
                Message = "Too many requests. Only allow 20 req per 10-seconds. Please try again later."
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
            return cfIp.FirstOrDefault() ?? UnknownIP;
        }

        // X-Forwarded-For
        if (context.Request.Headers.TryGetValue("X-Forwarded-For", out var forwardedFor))
        {
            return forwardedFor.ToString().Split(',').FirstOrDefault()?.Trim() ?? UnknownIP;
        }

        // localhost
        return context.Connection.RemoteIpAddress?.ToString() ?? UnknownIP;
    }
}