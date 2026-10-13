using BuildingBlocks.Messaging.Events;
using Inventory.Contracts;
using MassTransit;

namespace Ordering.Application.Features.Orders.Sagas
{
    public sealed class OrderCheckoutStateMachine : MassTransitStateMachine<OrderCheckoutSagaState>
    {
        public State AwaitingStockReservations { get; private set; } = null!;
        public State AwaitingOrderCancellation { get; private set; } = null!;

        public Event<OrderPlacedIntegrationEvent> OrderPlacedReceived { get; private set; } = null!;
        public Event<StockReservedIntegrationEvent> StockReservedReceived { get; private set; } = null!;
        public Event<StockReserveFailedIntegrationEvent> StockReserveFailedReceived { get; private set; } = null!;
        public Event<OrderCanceledIntegrationEvent> OrderCanceledReceived { get; private set; } = null!;

        // Timeout
        public Schedule<OrderCheckoutSagaState, StockReservationTimeoutExpired> StockReservationTimeout { get; private set; } = null!;

        public OrderCheckoutStateMachine()
        {
            InstanceState(x => x.CurrentState);

            Event(() => OrderPlacedReceived, eventConfig =>
            {
                eventConfig.CorrelateById(context => context.Message.OrderId);
                eventConfig.SelectId(context => context.Message.OrderId);
            });

            Event(() => StockReservedReceived, eventConfig =>
                eventConfig.CorrelateById(context => context.Message.SagaCorrelationId));

            Event(() => StockReserveFailedReceived, eventConfig =>
                eventConfig.CorrelateById(context => context.Message.SagaCorrelationId));

            Event(() => OrderCanceledReceived, eventConfig =>
                eventConfig.CorrelateById(context => context.Message.OrderId));

            Schedule(() => StockReservationTimeout, x => x.StockTimeoutTokenId, scheduleConfig =>
            {
                scheduleConfig.Delay = TimeSpan.FromSeconds(5);
                scheduleConfig.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

            Initially(
                When(OrderPlacedReceived)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.TotalAmount = context.Message.TotalPrice;
                        context.Saga.StartedAtUtc = DateTime.UtcNow;
                    })
                    .Publish(context =>
                         new ReserveStockIntegrationEvent(
                                context.Saga.CorrelationId,
                                context.Saga.OrderId,
                                context.Message.OrderItems))
                    .Schedule(StockReservationTimeout, context => new StockReservationTimeoutExpired(context.Saga.OrderId))
                    .TransitionTo(AwaitingStockReservations));

            During(AwaitingStockReservations,
                When(StockReservedReceived).Finalize(),
                When(StockReserveFailedReceived) // stok rezervasyonu başarısız olduğunda, stoğu release edip siparişi cancel etmeliyiz. yani compensation
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.FailureReason = context.Message.FailureReason;
                        context.Saga.CompletedAtUtc = DateTime.UtcNow;
                    })
                    .ThenAsync(async context =>
                    {
                        await context.Publish(
                            new CancelOrderIntegrationEvent(
                                context.Saga.CorrelationId,
                                context.Saga.OrderId,
                                context.Saga.FailureReason ?? "Stock reservation failed."),
                            context.CancellationToken);
                    })
                    .TransitionTo(AwaitingOrderCancellation),

                When(StockReservationTimeout.Received)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.FailureReason = "Stock reservation timeout.";
                        context.Saga.CompletedAtUtc = DateTime.UtcNow;
                    })
                    .Publish(context => new CancelOrderIntegrationEvent(
                                context.Saga.CorrelationId,
                                context.Saga.OrderId,
                                context.Saga.FailureReason ?? "Stock reservation timeout."))
                    .Publish(context => new ReleaseReserveIntegrationEvent(
                                context.Saga.CorrelationId,
                                context.Saga.OrderId))

                    .TransitionTo(AwaitingOrderCancellation));

            During(AwaitingOrderCancellation,
                When(OrderCanceledReceived).Finalize());

            //SetCompletedWhenFinalized(); // block hard delete 
        }
    }
}