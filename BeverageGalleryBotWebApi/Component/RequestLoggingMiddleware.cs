namespace BeverageGalleryBotWebApi.Component;

public class RequestLoggingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestLoggingMiddleware> _logger;

    public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Middle ware for record user ip.
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    public async Task Invoke(HttpContext context)
    {
        var ip = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
        var uIp = context.Request.Headers["X-Forwarded-For"].FirstOrDefault() ?? context.Connection.RemoteIpAddress?.ToString();
        var path = context.Request.Path;

        IHeaderDictionary headers = context.Request.Headers;
        this._logger.LogInformation("===Request Headers===");
        foreach (var header in headers)
        {
            this._logger.LogInformation("Header:[{Header}], Value:[{Value}]", header.Key.ToString(), header.Value.ToString());
        }
        this._logger.LogInformation("===Request Headers End.===");

        this._logger.LogInformation("Incoming request from public: [{IP}], user ip: [{UIP}] to {Path}", ip, uIp, path);

        await _next(context); // Call the next middleware
    }
}
