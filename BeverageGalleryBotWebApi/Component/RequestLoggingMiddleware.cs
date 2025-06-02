namespace BeverageGalleryBotWebApi.Component;

public class RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
{
    /// <summary>
    /// Middle ware for record user ip.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        logger.LogInformation("RAW Host header: {Host}", context.Request.Headers["Host"]);
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var uIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString();
        var path = context.Request.Path;
        
        var traceId = context.Request.Headers.TryGetValue("TraceId", out var traceHeader)
            ? traceHeader.FirstOrDefault() ?? Guid.NewGuid().ToString()
            : Guid.NewGuid().ToString();
        
        // log request trace id
        context.Request.Headers["TraceId"] = traceId;
        
        IHeaderDictionary headers = context.Request.Headers;
        logger.LogInformation("===Request Headers===");
        foreach (var header in headers)
        {
            logger.LogInformation("Header:[{Header}], Value:[{Value}]", header.Key.ToString(), header.Value.ToString());
        }
        logger.LogInformation("Incoming request from public: [{IP}], user ip: [{UIP}] to {Path}", ip, uIp, path);
        
        // log request trace id
        context.Response.Headers["TraceId"] = traceId;
        await next(context); // Call the next middleware
    }
}
