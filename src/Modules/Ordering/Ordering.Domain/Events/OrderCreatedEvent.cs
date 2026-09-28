namespace Ordering.Domain.Events
{
    public record OrderCreatedEvent(Guid OrderId, Guid CustomerId, string OrderName) : IDomainEvent;
}
