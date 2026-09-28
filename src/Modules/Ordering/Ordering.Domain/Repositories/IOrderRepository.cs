using Ordering.Domain.Entities;

namespace Ordering.Domain.Repositories
{
    public interface IOrderRepository
    {
        void Add(Order order);
        void Update(Order order);
        Task<Order?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Order>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<long> CountAsync(CancellationToken cancellationToken = default);
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
