using BeverageGalleryBotWebApi.Model;
using BeverageGalleryBotWebApi.Model.safequery;
using BeverageGalleryBotWebApi.Provider;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;
using Newtonsoft.Json;

namespace BeverageGalleryBotWebApi.Controllers;

[Route("api/[controller]")]
[ApiController]
public class BeverageController(ILogger<BeverageController> logger, MongoDBClientProvider mongoDbClient) : ControllerBase
{
    private ILogger<BeverageController> logger = logger;

    private MongoDBClientProvider mongoDbClient = mongoDbClient;
    
    [HttpPost("query")]
    public async Task<IActionResult> BaseQuery([FromBody] BeverageQueryParam queryParam)
    {
        if (queryParam.Limit < 0 || queryParam.Limit > 1000)
        {
            return BadRequest(
                new
                {
                    status = "error",
                    Message = "The limit must be between 0 and 1000 (inclusive)."
                });
        }

        var json = JsonConvert.SerializeObject(queryParam, Formatting.Indented);
        this.logger.LogInformation("===Query===");
        this.logger.LogInformation(json);
        try
        {
            var filters = new List<FilterDefinition<BeverageCollection>>();
            var builder = Builders<BeverageCollection>.Filter;

            queryParam.Conditions = queryParam.Conditions.ToDictionary(
                kv => kv.Key.Trim(),
                kv => kv.Value?.Trim() ?? string.Empty
            );

            foreach (var kv in queryParam.Conditions)
            {
                if (BeverageFieldQueryMap.QueryMap.TryGetValue(kv.Key, out var fn))
                {
                    filters.Add(fn(kv.Value));
                }
            }

            var finalFilter = filters.Any() ? builder.And(filters) : builder.Empty;
            
            var totalCount = await this.mongoDbClient.BeverageCollection.CountDocumentsAsync(finalFilter);
            
            var query = this.mongoDbClient.BeverageCollection
                .Find(finalFilter)
                .Skip(queryParam.Skip)
                .Limit(queryParam.Limit);

            var result = await query.ToListAsync();

            return Ok(new
            {
                TotalCount = totalCount,
                Skip = queryParam.Skip,
                Limit = queryParam.Limit,
                Values = result
            });
        }
        catch (Exception ex)
        {
            return BadRequest(
                new
                {
                    Status = "error",
                    Message = ex.Message
                });
        }
    }
}