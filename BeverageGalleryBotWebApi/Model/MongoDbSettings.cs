namespace BeverageGalleryBotWebApi.Model;

public class MongoDbSettings
{
    public string MongoDbEndpoint { get; set; } = string.Empty;
    public string BeverageDbName { get; set; } = string.Empty;
    public string BeverageCollectionName { get; set; } = string.Empty;
}