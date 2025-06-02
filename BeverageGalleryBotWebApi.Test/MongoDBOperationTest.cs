using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson;
using MongoDB.Bson.IO;
using MongoDB.Driver;
using Serilog;

namespace BeverageGalleryBotWebApi.Test;

[TestClass]
public class MongoDBOperationTest
{
    private const string MongoConnectionUrl = "";

    private static MongoClient client;

    private static IMongoDatabase mdb;

    public TestContext TestContext { get; set; }

    [ClassInitialize]
    public static void ClassInitialize(TestContext testContext)
    {
        Log.Logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Console()
            .WriteTo.File("TestLogs/testlog-.log", rollingInterval: RollingInterval.Day)
            .CreateLogger();

        client = new MongoClient(MongoConnectionUrl);
        mdb = client.GetDatabase("beverage");
    }

    [TestMethod]
    public async Task ClientConnectionTest()
    {
        IAsyncCursor<BsonDocument> collections = await mdb.ListCollectionsAsync();

        var collectionList = await collections.ToListAsync(); // 手动取出所有集合
        int index = 0;

        foreach (var document in collectionList)
        {
            string collectionName = document["name"].AsString;

            Log.Information($"[{index}] Collection: {collectionName}");

            // 获取集合
            var collection = mdb.GetCollection<BsonDocument>(collectionName);

            // 查询前5条
            var docs = await collection.Find(new BsonDocument()).Limit(5).ToListAsync();

            foreach (var doc in docs)
            {
                var json = doc.ToJson(new JsonWriterSettings
                {
                    OutputMode = JsonOutputMode.RelaxedExtendedJson
                });
                Log.Information(json);
            }

            index++;
        }
    }

    [TestMethod]
    public async Task TestQueryById()
    {
        string objId = "683c7e599c03daa2061e6f9c";

        var collection = mdb.GetCollection<BsonDocument>("beverage_list");

        var objectId = new ObjectId(objId);

        var filter = Builders<BsonDocument>.Filter.Eq("_id", objectId);
        var result = await collection.Find(filter).FirstOrDefaultAsync();

        var json = result.ToJson(new JsonWriterSettings
        {
            OutputMode = JsonOutputMode.RelaxedExtendedJson
        });
        Log.Information("Id: [{ID}], document:", objId);
        Log.Information(json);
    }
}