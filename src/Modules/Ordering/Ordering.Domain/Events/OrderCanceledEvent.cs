namespace Ordering.Domain.Events
{
    public record OrderCanceledEvent(Guid OrderId, string Reason) : IDomainEvent;
}