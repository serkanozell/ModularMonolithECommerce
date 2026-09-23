using BuildingBlocks.Shared.DDD;

namespace Basket.Domain.Events
{
    public record BasketItemRemovedEvent(Guid BasketId, Guid ProductId) : IDomainEvent;
}
