using BuildingBlocks.Shared.DDD;
using Inventory.Domain.ValueObjects;

namespace Inventory.Domain.Events
{
    public record StockAvailabilityChangedEvent(InventoryItemId InventoryItemId,
                                                ProductId ProductId,
                                                bool IsInStock,
                                                int AvailableQuantity) : IDomainEvent;
}