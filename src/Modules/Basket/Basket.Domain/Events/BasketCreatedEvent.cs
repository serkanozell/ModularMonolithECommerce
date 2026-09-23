using BuildingBlocks.Shared.DDD;

namespace Basket.Domain.Events
{
    public record BasketCreatedEvent(Guid BasketId, string UserName) : IDomainEvent;
}
