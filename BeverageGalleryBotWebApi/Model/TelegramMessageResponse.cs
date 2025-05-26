using Newtonsoft.Json;

namespace BeverageGalleryBotWebApi.Model;

public class TelegramMessageResponse
{
    [JsonProperty("ok")]
    public bool Ok { get; set; } = false;

    [JsonProperty("result")]
    public TelegramMessageResult Result { get; set; } = new();
}

