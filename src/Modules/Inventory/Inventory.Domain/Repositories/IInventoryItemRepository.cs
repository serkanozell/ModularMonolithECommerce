using Inventory.Domain.Entities;

namespace Inventory.Domain.Repositories
{
    public interface IInventoryItemRepository
    {
        void Add(InventoryItem item);

        void Update(InventoryItem item);

        Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<InventoryItem?> GetByIdWithReservationsAsync(Guid id, Guid orderId, CancellationToken cancellationToken = default);

        Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<InventoryItem>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<long> CountAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}