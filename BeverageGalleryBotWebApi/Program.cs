using BeverageGalleryBotWebApi.Model;
using Microsoft.Extensions.Options;
using Serilog;
using Telegram.Bot;

namespace BeverageGalleryBotWebApi;

public class Program
{
    public static void Main(string[] args)
    {

        // logging
        Log.Logger = new LoggerConfiguration()
            .WriteTo.Console() // Console
            .WriteTo.File(
                "Logs/BackendLog-.log",
                rollingInterval: RollingInterval.Day, // Roll log by day.
                outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss} [{Level:u3}] {Message}{NewLine}{Exception}"
            )
            .MinimumLevel.Information()
            .CreateLogger();

        var builder = WebApplication.CreateBuilder(args);

        // Use Serilog
        builder.Host.UseSerilog();

        // appsettings.json
        builder.Services.Configure<BotSetting>(
            builder.Configuration.GetSection("BotSetting")
        );

        builder.Services.AddSingleton<ITelegramBotClient>(sp =>
        {
            var options = sp.GetRequiredService<IOptions<BotSetting>>().Value;
            return new TelegramBotClient(options.BotToken);
        });

        builder.Services.AddHttpClient();
        builder.Services.AddControllers();

        WebApplication app = builder.Build();

        app.MapControllers();

        app.Run();
    }
}