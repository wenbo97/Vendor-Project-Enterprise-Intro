using BeverageGalleryBotWebApi.Component;
using BeverageGalleryBotWebApi.Model;
using BeverageGalleryBotWebApi.Provider;
using BeverageGalleryBotWebApi.Service;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.Extensions.Options;
using Serilog;
using StackExchange.Redis;
using Telegram.Bot;

namespace BeverageGalleryBotWebApi;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Load config
        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Setup Serilog
        builder.Host.UseSerilog((context, services, configuration) =>
        {
            configuration
                .ReadFrom.Configuration(context.Configuration)
                .ReadFrom.Services(services)
                .Enrich.FromLogContext()
                .WriteTo.Console()
                .WriteTo.File(
                    "Logs/BackendLog-.log",
                    rollingInterval: RollingInterval.Day,
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}"
                );
        });

        // Setup Kestrel
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5003); // HTTP
            // serverOptions.ListenLocalhost(5001, listenOptions => { listenOptions.UseHttps(); });
        });

        // Load Bot and Mongo Settings
        builder.Services.Configure<BotSetting>(builder.Configuration.GetSection("BotSetting"));
        builder.Services.Configure<MongoDbSettings>(builder.Configuration.GetSection("MongoDBSetting"));
        builder.Services.Configure<RedisOptions>(builder.Configuration.GetSection("RedisSetting"));

        // Telegram Bot Client
        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BotSetting>>().Value;
            return new TelegramBotClient(options.BotToken);
        });
        
        // Custom services
        builder.Services.AddSingleton<MongoDBClientProvider>();
        builder.Services.AddSingleton<IConnectionMultiplexer>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<RedisOptions>>().Value;
            return ConnectionMultiplexer.Connect(options.RedisEndpoint);
        });
        
        builder.Services.Configure<ForwardedHeadersOptions>(options =>
        {
            options.KnownNetworks.Clear();
            options.KnownProxies.Clear();
        });
        
        // Redis limiter
        builder.Services.AddSingleton<RedisRateLimiter>();
        
        // Add controllers
        builder.Services.AddHttpClient();
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAll", builder =>
            {
                builder
                    .SetIsOriginAllowed(origin =>
                        origin == "https://snusfactorycn.com" ||
                        origin == "https://www.snusfactorycn.com" ||
                        origin == "https://api.snusfactorycn.com"
                    )
                    .AllowAnyHeader()
                    .AllowAnyMethod()
                    .AllowCredentials();
            });
        });

        builder.Services.AddControllers().AddNewtonsoftJson();

        // Build app
        var app = builder.Build();

        app.UseForwardedHeaders(new ForwardedHeadersOptions
        {
            ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto,
            ForwardLimit = 2
        });

        app.UseRouting();                     // Enable routing
        app.UseCors("AllowAll");              // Enable CORS
        app.UseSerilogRequestLogging();       // Enable Serilog
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseMiddleware<RequestRateLimitingMiddleware>();

        // app.UseAuthorization();

        app.MapGet("/", () => "BeverageGallery service API is running.");
        app.MapControllers();
        app.Run();
    }
}
