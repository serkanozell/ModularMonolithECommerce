using BuildingBlocks.Shared.DDD;

namespace Inventory.Domain.Events
{
    public record StockChangedEvent(Guid InventoryItemId, Guid ProductId, int PreviousQuantity, int CurrentQuantity) : IDomainEvent;
}
