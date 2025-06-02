using Microsoft.Extensions.Options;
using BeverageGalleryBotWebApi.Model;
using MongoDB.Driver;

namespace BeverageGalleryBotWebApi.Provider;

/// <summary>
/// Mongodb provider
/// </summary>
public class MongoDBClientProvider
{
    private readonly ILogger<MongoDBClientProvider> logger;
    private readonly IMongoDatabase database;
    private readonly MongoDbSettings dbSettings;

    public MongoDBClientProvider(ILogger<MongoDBClientProvider> logger, IOptions<MongoDbSettings> dbSettingsOptions)
    {
        this.logger = logger;
        this.dbSettings = dbSettingsOptions.Value;

        var client = new MongoClient(this.dbSettings.MongoDbEndpoint);
        this.database = client.GetDatabase(this.dbSettings.BeverageDbName);

        this.logger.LogInformation(
            "Connected mongo server: [{Endpoint}], db: [{DBName}], collection: [{Collection}]",
            this.dbSettings.MongoDbEndpoint, this.dbSettings.BeverageDbName, this.dbSettings.BeverageCollectionName);
    }

    public IMongoCollection<BeverageCollection> BeverageCollection =>
        this.database.GetCollection<BeverageCollection>(this.dbSettings.BeverageCollectionName);
}
