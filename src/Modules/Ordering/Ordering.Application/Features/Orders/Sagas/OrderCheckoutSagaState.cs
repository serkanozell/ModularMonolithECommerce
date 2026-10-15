using MassTransit;

namespace Ordering.Application.Features.Orders.Sagas
{
    public sealed class OrderCheckoutSagaState : SagaStateMachineInstance
    {
        public Guid CorrelationId { get; set; }
        public string CurrentState { get; set; } = null!;
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string? FailureReason { get; set; }
        public DateTime StartedAtUtc { get; set; }
        public DateTime? CompletedAtUtc { get; set; }

        public Guid? StockTimeoutTokenId { get; set; }
        public Guid? SagaTimeoutTokenId { get; set; }
    }
}