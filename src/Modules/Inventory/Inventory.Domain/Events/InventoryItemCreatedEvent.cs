using BuildingBlocks.Shared.DDD;
using Inventory.Domain.ValueObjects;

namespace Inventory.Domain.Events
{
    public record InventoryItemCreatedEvent(InventoryItemId InventoryItemId,
                                            ProductId ProductId,
                                            int InitialQuantity) : IDomainEvent;
}