using Newtonsoft.Json;

namespace BeverageGalleryBotWebApi.Model;

public class TelegramUser
{
    [JsonProperty("id")]
    public long Id { get; set; } = long.MinValue;

    [JsonProperty("is_bot")]
    public bool IsBot { get; set; } = false;

    [JsonProperty("first_name")]
    public string FirstName { get; set; } = string.Empty;

    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;
}
