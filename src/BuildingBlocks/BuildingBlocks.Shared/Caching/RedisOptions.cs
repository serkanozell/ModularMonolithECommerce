namespace BuildingBlocks.Shared.Caching
{
    public class RedisOptions
    {
        public int DefaultAbsoluteExpirationInMinutes { get; set; } = 60;
        public int DefaultSlidingExpirationInMinutes { get; set; } = 30;
    }
}