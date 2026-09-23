using BuildingBlocks.Shared.DDD;

namespace Basket.Domain.Events
{
    public record BasketItemAddedEvent(Guid BasketId,
                                       Guid ProductId,
                                       int Quantity,
                                       decimal Price,
                                       string ProductName) : IDomainEvent;
}