using Ordering.Domain.Repositories;
using Ordering.Infrastructure.Persistence;

namespace Ordering.Infrastructure.Repositories
{
    public class OrderRepository(OrderingDbContext context) : IOrderRepository
    {
        public void Add(Order order) => context.Orders.Add(order);

        public void Update(Order order) => context.Orders.Update(order);

        public Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            context.Orders
                   .Include(o => o.Items)
                   .FirstOrDefaultAsync(o => o.Id == id && !o.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<Order>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
            await ActiveOrders()
                  .Include(o => o.Items)
                  .AsNoTracking()
                  .OrderByDescending(o => o.CreatedAt)
                  .Skip(pageNumber * pageSize)
                  .Take(pageSize)
                  .ToListAsync(cancellationToken);

        public Task<long> CountAsync(CancellationToken cancellationToken = default) =>
            ActiveOrders().LongCountAsync(cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);

        private IQueryable<Order> ActiveOrders() =>
            context.Orders.Where(o => !o.IsDeleted && o.IsActive);
    }
}
