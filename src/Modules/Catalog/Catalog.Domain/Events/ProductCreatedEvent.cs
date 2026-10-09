using BuildingBlocks.Shared.DDD;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Events
{
    public record ProductCreatedEvent(ProductId ProductId,
                                      ProductName Name,
                                      List<string> Category,
                                      string? Description,
                                      decimal Price,
                                      int StockQuantity) : IDomainEvent;
}