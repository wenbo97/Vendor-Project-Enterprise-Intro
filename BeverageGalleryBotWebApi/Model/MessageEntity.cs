using Newtonsoft.Json;
using Telegram.Bot.Types.ReplyMarkups;

namespace BeverageGalleryBotWebApi.Model;

public class MessageEntity
{
    [JsonProperty("offset")]
    public int Offset { get; set; } = int.MinValue;

    [JsonProperty("length")]
    public int Length { get; set; } = int.MinValue;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;
}

public class LinkPreviewOptions
{
    [JsonProperty("is_disabled")]
    public bool IsDisabled { get; set; } = false;
}

public class ReplyMarkup
{
    [JsonProperty("inline_keyboard")]
    public List<List<InlineKeyboardButton>> InlineKeyboard { get; set; } = new();
}

public class InlineKeyboardButton
{
    [JsonProperty("text")]
    public string Text { get; set; } = string.Empty;

    [JsonProperty("url")]
    public string Url { get; set; } = string.Empty;

    [JsonProperty("callback_data")]
    public string CallbackData { get; set; } = string.Empty;
}