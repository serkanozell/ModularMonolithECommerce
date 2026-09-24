using Basket.Domain.Helpers;
using Basket.Domain.Repositories;
using BuildingBlocks.Shared.Caching;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using System.Text.Json;

namespace Basket.Infrastructure.Repositories
{
    public class CachedBasketRepository(IBasketRepository repository, IDistributedCache cache, IOptions<RedisOptions> redisOptions) : IBasketRepository
    {
        private readonly DistributedCacheEntryOptions cacheEntryOptions = new()
        {
            AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(redisOptions.Value.DefaultAbsoluteExpirationInMinutes),
            SlidingExpiration = TimeSpan.FromMinutes(redisOptions.Value.DefaultSlidingExpirationInMinutes)
        };

        private readonly JsonSerializerOptions jsonSerializerOptions = new()
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = true,
            Converters =
            {
                new ShoppingCartConverter(),new ShoppingCartItemConverter()
            }
        };

        private static string BasketKey(string userName) => $"basket:{userName}";

        public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            await repository.CreateBasket(basket, cancellationToken);

            await cache.SetStringAsync(BasketKey(basket.UserName), JsonSerializer.Serialize(basket, jsonSerializerOptions), cacheEntryOptions, cancellationToken);

            return basket;
        }

        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            await repository.DeleteBasket(userName, cancellationToken);

            await cache.RemoveAsync(BasketKey(userName), cancellationToken);

            return true;
        }

        public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            if (!asNoTracking)
            {
                return await repository.GetBasket(userName, false, cancellationToken);
            }
            var basketFromCache = await cache.GetStringAsync(BasketKey(userName), cancellationToken);

            if (!string.IsNullOrEmpty(basketFromCache))
                return JsonSerializer.Deserialize<ShoppingCart>(basketFromCache, jsonSerializerOptions)!;

            var basketfromDb = await repository.GetBasket(userName, true, cancellationToken);

            await cache.SetStringAsync(BasketKey(userName), JsonSerializer.Serialize(basketfromDb, jsonSerializerOptions), cacheEntryOptions, cancellationToken);

            return basketfromDb;
        }

        public Task<IReadOnlyList<ShoppingCart>> GetBasketsByProductId(Guid productId, CancellationToken cancellationToken = default)
        {
            return repository.GetBasketsByProductId(productId, cancellationToken);
        }

        public async Task<int> UpdateBaskets(IEnumerable<ShoppingCart> baskets, CancellationToken cancellationToken = default)
        {
            var basketList = baskets.ToList();

            var result = await repository.UpdateBaskets(basketList, cancellationToken);

            foreach (var basket in basketList)
                await cache.RemoveAsync(BasketKey(basket.UserName), cancellationToken);

            return result;
        }

        public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
        {
            var result = await repository.SaveChangesAsync(userName, cancellationToken);

            if (!string.IsNullOrEmpty(userName))
                await cache.RemoveAsync(BasketKey(userName), cancellationToken);

            return result;
        }
    }
}