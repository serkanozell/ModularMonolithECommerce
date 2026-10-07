using BuildingBlocks.Shared.DDD;

namespace Inventory.Domain.Events
{
    public record StockAvailabilityChangedEvent(Guid InventoryItemId,
                                                Guid ProductId,
                                                bool IsInStock,
                                                int AvailableQuantity) : IDomainEvent;
}