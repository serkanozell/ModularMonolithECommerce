using Basket.Domain.Exceptions;
using Basket.Domain.Repositories;
using Basket.Infrastructure.Persistence;

namespace Basket.Infrastructure.Repositories
{
    public class BasketRepository(BasketDbContext dbContext) : IBasketRepository
    {
        public async Task<ShoppingCart> GetBasket(string userName, bool asNoTracking = true, CancellationToken cancellationToken = default)
        {
            var query = dbContext.ShoppingCarts
                .Include(sc => sc.Items)
                .Where(sc => sc.UserName == userName
                            && sc.IsActive
                            && !sc.IsDeleted);

            if (asNoTracking)
                query = query.AsNoTracking();

            var basket = await query.FirstOrDefaultAsync(cancellationToken);

            return basket ?? throw new BasketNotFoundException(userName);
        }

        public async Task<ShoppingCart> CreateBasket(ShoppingCart basket, CancellationToken cancellationToken = default)
        {
            dbContext.ShoppingCarts.Add(basket);
            await dbContext.SaveChangesAsync(cancellationToken);
            return basket;
        }

        public async Task<bool> DeleteBasket(string userName, CancellationToken cancellationToken = default)
        {
            var basket = await GetBasket(userName, false, cancellationToken);

            dbContext.ShoppingCarts.Remove(basket);
            await dbContext.SaveChangesAsync(cancellationToken);

            return true;
        }

        public async Task<IEnumerable<ShoppingCartItem>> GetBasketItemsByProductId(Guid productId, CancellationToken cancellationToken = default)
        {
            return await dbContext.ShoppingCarts
                .SelectMany(x => x.Items)
                .Where(i => i.ProductId == productId
                            && i.IsActive
                            && !i.IsDeleted)
                .ToListAsync(cancellationToken);
        }

        public async Task<int> SaveChangesAsync(string? userName = null, CancellationToken cancellationToken = default)
        {
            return await dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}