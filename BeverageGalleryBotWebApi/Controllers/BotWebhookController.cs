using BeverageGalleryBotWebApi.Content;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics.Contracts;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeverageGalleryBotWebApi.Controllers;


[Route("api/[controller]")]
[ApiController]
public class BotWebhookController : ControllerBase
{
    private const string UnkownUsr = "Unknown";

    private ILogger<BotWebhookController> logger;


    private readonly ITelegramBotClient botClient;

    public BotWebhookController(ILogger<BotWebhookController> logger, ITelegramBotClient botClient)
    {
        this.logger = logger;
        this.botClient = botClient;
    }

    /// <summary>
    /// Webhook endpoint for receiving Telegram update messages.
    /// </summary>
    /// <param name="update">Telegram update payload sent via webhook.</param>
    /// <returns>HTTP 200 OK if processed successfully.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] Update update)
    {
        try
        {
            if (update == null)
            {
                this.logger.LogWarning("Received null update.");
                return BadRequest();
            }
            Message botMessage = update.Message ?? throw new InvalidOperationException("Received null update.Message.");

            string usr = botMessage.From?.ToString() ?? UnkownUsr;

            await botClient.SendChatAction(botMessage.Chat.Id, ChatAction.Typing);
            await Task.Delay(1500);


            UpdateType type = update.Type;

            switch (type)
            {
                case UpdateType.Message:
                    await this.CallPop(botMessage, usr);
                    break;
                case UpdateType.InlineQuery:
                    await this.CallPing(update);
                    break;
                default: break;
            }

            return Ok("ok");
        }
        catch (Exception ex)
        {
            this.logger.LogError(ex, string.Format("Error processing Telegram update, ExMsg: [{0}]", ex.Message));
            return StatusCode(500, "Internal server error");
        }
    }

    private async Task CallPing(Update update)
    {
        var contactResult = new InlineQueryResultArticle(
                   id: Guid.NewGuid().ToString(),
                   title: "💬 联系客服A",
                   inputMessageContent: new InputTextMessageContent
                   {

                       MessageText = """
                    
                    <b>客服联系方式</b>
                    Telegram: @customer1
                    微信: <code>wine_support</code>
                    
                    """,
                       ParseMode = ParseMode.Html,
                       
                   })
        {
            Description = "点击查看客服联系方式",
            ReplyMarkup = new InlineKeyboardMarkup(new[]
                   {
                        InlineKeyboardButton.WithUrl("🔗 打开 Telegram 用户", "https://t.me/customer1"),
                        InlineKeyboardButton.WithUrl("🛒 立即购买", "https://yourshop.com/item_123")
                    })
        };


        await botClient.AnswerInlineQuery(update.InlineQuery.Id, new[]
        {
            contactResult
        });
    }

    private async Task CallPop(Message botMessage, string usr)
    {
        this.logger.LogInformation($"Received message: [{botMessage.Text}] from {usr}");
        await this.botClient.SendMessage(
            chatId: botMessage.Chat.Id,
            text: ChatContent.WelCome,
            parseMode: ParseMode.Html,
            replyMarkup: new InlineKeyboardMarkup(new[]
            {
                        new[]{
                            InlineKeyboardButton.WithUrl("💬 联系客服A", "https://t.me/c1"),
                            InlineKeyboardButton.WithUrl("💬 联系客服B", "https://t.me/c2"),
                        },
                        new []
                        {
                            InlineKeyboardButton.WithUrl("🛒 立即购买", "https://yourshop.com/item_123"),
                        }
            }));
    }
}
