using BeverageGalleryBotWebApi.Model;
using Microsoft.AspNetCore.Mvc;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BeverageGalleryBotWebApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class BotWebhookController : ControllerBase
{
    private ILogger<BotWebhookController> logger;


    private readonly ITelegramBotClient botClient;

    public BotWebhookController(ILogger<BotWebhookController> logger, ITelegramBotClient botClient)
    {
        this.logger = logger;
        this.botClient = botClient;
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        if (update.Type == UpdateType.Message && update.Message?.Text != null)
        {
            var msg = update.Message;
            this.logger.LogInformation($"Received message: {msg.Text} from {msg.Chat.Id}");

            if (msg.Text.StartsWith("/start"))
            {
                var args = msg.Text.Length > 7 ? msg.Text.Substring(7).Trim() : "(no args)";
                await this.botClient.SendMessage(
                    chatId: msg.Chat.Id,
                    text: $"👋 Welcome! You launched me with argument: {args}"
                );
            }
        }

        return Ok();
    }
}
