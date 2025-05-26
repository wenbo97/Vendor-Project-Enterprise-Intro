using Newtonsoft.Json;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeverageGalleryBotWebApi.Model;

public class TelegramMessageResult
{
    [JsonProperty("message_id")]
    public int MessageId { get; set; } = int.MinValue;

    [JsonProperty("from")]
    public TelegramUser From { get; set; } = new();

    [JsonProperty("chat")]
    public TelegramChat Chat { get; set; } = new();
    [JsonProperty("date")]
    public long Date { get; set; } = long.MinValue;

    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("entities")]
    public List<MessageEntity> Entities { get; set; } = new ();

    [JsonProperty("link_preview_options")]
    public LinkPreviewOptions LinkPreviewOptions { get; set; } = new();

    [JsonProperty("reply_markup")]
    public ReplyMarkup ReplyMarkup { get; set; } = new();
}
