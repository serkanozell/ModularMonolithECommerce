using BuildingBlocks.Shared.DDD;

namespace Inventory.Domain.Events
{
    public record InventoryItemCreatedEvent(Guid InventoryItemId, Guid ProductId, int InitialQuantity) : IDomainEvent;
}
