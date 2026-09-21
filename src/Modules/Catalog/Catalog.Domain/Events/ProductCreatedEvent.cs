using BuildingBlocks.Shared.DDD;

namespace Catalog.Domain.Events
{
    public record ProductCreatedEvent(Guid ProductId,
                                      string Name,
                                      List<string> Category,
                                      string? Description,
                                      decimal Price,
                                      int StockQuantity) : IDomainEvent;
}