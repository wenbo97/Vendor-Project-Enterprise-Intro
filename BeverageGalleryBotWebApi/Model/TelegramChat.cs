using Newtonsoft.Json;
using Telegram.Bot.Types;

namespace BeverageGalleryBotWebApi.Model;

public class TelegramChat
{
    [JsonProperty("id")]
    public long Id { get; set; } = long.MinValue;

    [JsonProperty("title")]
    public string Title { get; set; } = string.Empty;

    [JsonProperty("type")]
    public string Type { get; set; } = string.Empty;

    [JsonProperty("all_members_are_administrators")]
    public bool AllMembersAreAdministrators { get; set; } = false;

    [JsonProperty("accepted_gift_types")]
    public AcceptedGiftTypes AcceptedGiftTypes { get; set; } = new ();
}
