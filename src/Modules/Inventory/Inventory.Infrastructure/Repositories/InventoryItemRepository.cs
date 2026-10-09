using Inventory.Domain.Repositories;
using Inventory.Infrastructure.Persistence;

namespace Inventory.Infrastructure.Repositories
{
    public class InventoryItemRepository(InventoryDbContext context) : IInventoryItemRepository
    {
        public void Add(InventoryItem item) => context.InventoryItems.Add(item);

        public void Update(InventoryItem item) => context.InventoryItems.Update(item);

        public async Task<InventoryItem?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            await context.InventoryItems.FirstOrDefaultAsync(item => item.Id == InventoryItemId.Of(id) && !item.IsDeleted, cancellationToken);

        public async Task<InventoryItem?> GetByIdWithReservationsAsync(Guid id, Guid orderId, CancellationToken cancellationToken = default) =>
            await context.InventoryItems
                .Include(item => item.Reservations.Where(reservation => reservation.OrderId == OrderId.Of(orderId) && reservation.IsActive && !reservation.IsDeleted))
                .FirstOrDefaultAsync(item => item.Id == InventoryItemId.Of(id) && !item.IsDeleted, cancellationToken);

        public async Task<InventoryItem?> GetByProductIdAsync(Guid productId, CancellationToken cancellationToken = default) =>
            await context.InventoryItems.FirstOrDefaultAsync(item => item.ProductId == ProductId.Of(productId) && !item.IsDeleted, cancellationToken);

        public async Task<IReadOnlyList<InventoryItem>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
            await ActiveItems()
                .AsNoTracking()
                .OrderByDescending(item => item.CreatedAt)
                .Skip(pageNumber * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

        public async Task<long> CountAsync(CancellationToken cancellationToken = default) =>
            await ActiveItems().LongCountAsync(cancellationToken);

        public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            await context.SaveChangesAsync(cancellationToken);

        private IQueryable<InventoryItem> ActiveItems() =>
            context.InventoryItems.Where(item => !item.IsDeleted && item.IsActive);
    }
}