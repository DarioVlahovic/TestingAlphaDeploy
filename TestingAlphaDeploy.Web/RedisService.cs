using StackExchange.Redis;

namespace TestingAlphaDeploy.Web;

public class RedisService
{
    private readonly IConnectionMultiplexer? _redis;
    private readonly ILogger<RedisService> _logger;

    public RedisService(IConfiguration configuration, ILogger<RedisService> logger)
    {
        _logger = logger;
        var connectionString = configuration["REDIS_CONNECTION"] ?? "localhost:6379";

        try
        {
            _redis = ConnectionMultiplexer.Connect(new ConfigurationOptions
            {
                EndPoints = { connectionString },
                AbortOnConnectFail = false,
                ConnectTimeout = 3000
            });
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Failed to connect to Redis at {ConnectionString}", connectionString);
        }
    }

    public async Task<bool> IsConnectedAsync()
    {
        try
        {
            if (_redis is null || !_redis.IsConnected) return false;
            var db = _redis.GetDatabase();
            await db.PingAsync();
            return true;
        }
        catch
        {
            return false;
        }
    }

    public async Task<TimeSpan?> PingAsync()
    {
        try
        {
            if (_redis is null || !_redis.IsConnected) return null;
            var db = _redis.GetDatabase();
            return await db.PingAsync();
        }
        catch
        {
            return null;
        }
    }

    public async Task<bool> SetValueAsync(string key, string value, TimeSpan? expiry = null)
    {
        try
        {
            if (_redis is null || !_redis.IsConnected) return false;
            var db = _redis.GetDatabase();
            return await db.StringSetAsync(key, value, expiry);
        }
        catch
        {
            return false;
        }
    }

    public async Task<string?> GetValueAsync(string key)
    {
        try
        {
            if (_redis is null || !_redis.IsConnected) return null;
            var db = _redis.GetDatabase();
            return await db.StringGetAsync(key);
        }
        catch
        {
            return null;
        }
    }

    public string GetConnectionInfo()
    {
        if (_redis is null) return "Not configured";
        return _redis.IsConnected
            ? $"Connected to {_redis.GetEndPoints().FirstOrDefault()}"
            : "Disconnected";
    }
}
