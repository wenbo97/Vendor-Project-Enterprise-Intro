namespace BeverageGalleryBotWebApi.Model;

public class BotSetting
{
    public string BotId { get; set; } = string.Empty;

    public string ApiEndpoint { get; set; } = string.Empty;

    public string BotToken { get; set; } = string.Empty;

    public long TestGroupId { get; set; } = long.MinValue;
}
