using Basket.Domain.Entities;

namespace Basket.Domain.Repositories
{
    public interface IBasketRepository
    {
        Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default);
        Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default);
        Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<ShoppingCart>> GetBasketsByProductId(Guid productId, CancellationToken cancellationToken = default);
        Task<int> UpdateBaskets(IEnumerable<ShoppingCart> baskets, CancellationToken cancellationToken = default);
    }
}