namespace Ordering.Domain.Events
{
    public record OrderCompletedEvent(Guid OrderId) : IDomainEvent;
}
