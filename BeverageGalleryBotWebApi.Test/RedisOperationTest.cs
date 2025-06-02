using System.Diagnostics;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using StackExchange.Redis;

namespace BeverageGalleryBotWebApi.Test;

[TestClass]
public sealed class RedisOperationTest
{
    public TestContext TestContext { get; set; }
    
    public async Task WaitUntilConnectedAsync(ConnectionMultiplexer redis, int timeoutMs = 5000)
    {
        int waited = 0;
        while (!redis.IsConnected)
        {
            if (waited >= timeoutMs)
                throw new TimeoutException("Redis connection timed out.");

            await Task.Delay(100);
            waited += 100;
        } 
        Debug.WriteLine("Redis has connected.");
    }
    
    [TestMethod]
    public async Task RedisConnection()
    {
        var redis = await ConnectionMultiplexer.ConnectAsync("localhost:6379");
        await this.WaitUntilConnectedAsync(redis);
        this.TestContext.WriteLine("Test finished.");
    }
}