using Newtonsoft.Json;

namespace BeverageGalleryBotWebApi.Model;

public class AcceptedGiftTypes
{
    [JsonProperty("unlimited_gifts")]
    public bool UnlimitedGifts { get; set; }

    [JsonProperty("limited_gifts")]
    public bool LimitedGifts { get; set; }

    [JsonProperty("unique_gifts")]
    public bool UniqueGifts { get; set; }

    [JsonProperty("premium_subscription")]
    public bool PremiumSubscription { get; set; }
}
