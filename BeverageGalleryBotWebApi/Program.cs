using BeverageGalleryBotWebApi.Component;
using BeverageGalleryBotWebApi.Model;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;

namespace BeverageGalleryBotWebApi;

public class Program
{
    public static void Main(string[] args)
    {

        var builder = WebApplication.CreateBuilder(args);

        builder.Configuration
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true)
            .AddEnvironmentVariables();

        // Use Serilog
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

        // appsettings.json
        builder.WebHost.ConfigureKestrel(serverOptions =>
        {
            serverOptions.ListenAnyIP(5000); // HTTP
            serverOptions.ListenAnyIP(5001, listenOptions =>
            {
                listenOptions.UseHttps();
            });
        });

        builder.Services.Configure<BotSetting>(builder.Configuration.GetSection("BotSetting"));

        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BotSetting>>().Value;
            return new TelegramBotClient(options.BotToken);
        });

        builder.Services.AddHttpClient();
        builder.Services.AddControllers();

        WebApplication app = builder.Build();

        app.MapControllers();
        app.UseSerilogRequestLogging();
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.MapGet("/", () => "BeverageGallery Bot API is running.");
        app.Run();
    }
}