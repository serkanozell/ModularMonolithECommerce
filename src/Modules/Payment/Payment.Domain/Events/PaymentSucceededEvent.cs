using BuildingBlocks.Shared.DDD;

namespace Payment.Domain.Events
{
    public record PaymentSucceededEvent() : IDomainEvent;
}