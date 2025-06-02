using StackExchange.Redis;

namespace BeverageGalleryBotWebApi.Service;

public class RedisRateLimiter(IConnectionMultiplexer redis, int maxQps = 2)
{
    private readonly IDatabase redisClient = redis.GetDatabase();
    private readonly TimeSpan window = TimeSpan.FromSeconds(10);// windows size
    private readonly int maxRequestsPerWindow = maxQps * 10; // max request count per ip in one window

    /// <summary>
    /// Limit the controller calling.
    /// </summary>
    /// <param name="ip">The string ip address</param>
    /// <returns></returns>
    public async Task<bool> IsLimitedAsync(string ip)
    {
        var now = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
        // Each 10-second interval is treated as one bucket
        var windowKey = now / 10;
        var key = $"rate_limit:{ip}:{windowKey}";
        var count = await this.redisClient.StringIncrementAsync(key);
        if (count == 1)
        {
            await this.redisClient.KeyExpireAsync(key, this.window);
        }
        
        return count > maxRequestsPerWindow;
    }
}