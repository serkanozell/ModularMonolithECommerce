namespace Ordering.Domain.Events
{
    public record OrderConfirmedEvent(Guid OrderId) : IDomainEvent;
}