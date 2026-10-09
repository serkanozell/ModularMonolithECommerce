using BuildingBlocks.Shared.DDD;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Events
{
    public record ProductPriceChangedEvent(ProductId ProductId,
                                           decimal OldPrice,
                                           decimal NewPrice) : IDomainEvent;
}