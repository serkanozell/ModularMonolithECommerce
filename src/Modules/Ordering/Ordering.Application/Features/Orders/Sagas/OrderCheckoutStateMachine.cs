using BuildingBlocks.Messaging.Events;
using Inventory.Contracts;
using MassTransit;

namespace Ordering.Application.Features.Orders.Sagas
{
    public sealed class OrderCheckoutStateMachine : MassTransitStateMachine<OrderCheckoutSagaState>
    {
        private static readonly TimeSpan StockReservationTimeoutDelay = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan SagaTimeoutDelay = TimeSpan.FromMinutes(5);

        // States
        public State AwaitingStockReservations { get; private set; } = null!;
        public State AwaitingOrderConfirmation { get; private set; } = null!;
        public State AwaitingStockConfirmation { get; private set; } = null!;
        public State AwaitingOrderCompletion { get; private set; } = null!;
        public State AwaitingOrderCancellation { get; private set; } = null!;

        // Events
        public Event<OrderPlacedIntegrationEvent> OrderPlacedReceived { get; private set; } = null!;
        public Event<StockReservedIntegrationEvent> StockReservedReceived { get; private set; } = null!;
        public Event<StockReserveFailedIntegrationEvent> StockReserveFailedReceived { get; private set; } = null!;
        public Event<OrderConfirmedIntegrationEvent> OrderConfirmedReceived { get; private set; } = null!;
        public Event<StockReservationConfirmedIntegrationEvent> StockReservationConfirmedReceived { get; private set; } = null!;
        public Event<StockReservationConfirmFailedIntegrationEvent> StockReservationConfirmFailedReceived { get; private set; } = null!;
        public Event<OrderCompletedIntegrationEvent> OrderCompletedReceived { get; private set; } = null!;
        public Event<OrderCanceledIntegrationEvent> OrderCanceledReceived { get; private set; } = null!;

        // Fault events (consumer retry'ları tükenince MassTransit Fault<T> yayınlar)
        public Event<Fault<ReserveStockIntegrationEvent>> ReserveStockFaulted { get; private set; } = null!;
        public Event<Fault<ConfirmOrderIntegrationEvent>> ConfirmOrderFaulted { get; private set; } = null!;
        public Event<Fault<ConfirmStockReservationIntegrationEvent>> ConfirmStockReservationFaulted { get; private set; } = null!;
        public Event<Fault<CompleteOrderIntegrationEvent>> CompleteOrderFaulted { get; private set; } = null!;
        public Event<Fault<CancelOrderIntegrationEvent>> CancelOrderFaulted { get; private set; } = null!;

        // Timeouts
        public Schedule<OrderCheckoutSagaState, StockReservationTimeoutExpired> StockReservationTimeout { get; private set; } = null!;
        public Schedule<OrderCheckoutSagaState, SagaTimeoutExpired> SagaTimeout { get; private set; } = null!;

        public OrderCheckoutStateMachine()
        {
            InstanceState(x => x.CurrentState);

            ConfigureEvents();
            ConfigureSchedules();

            // ---------------------------------------------------------------
            // Başlangıç
            // ---------------------------------------------------------------
            Initially(
                When(OrderPlacedReceived)
                    .Then(context =>
                    {
                        context.Saga.OrderId = context.Message.OrderId;
                        context.Saga.CustomerId = context.Message.CustomerId;
                        context.Saga.TotalAmount = context.Message.TotalPrice;
                        context.Saga.StartedAtUtc = DateTime.UtcNow;
                    })
                    .Publish(context => new ReserveStockIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId, context.Message.OrderItems))
                    .Schedule(StockReservationTimeout, context => new StockReservationTimeoutExpired(context.Saga.OrderId))
                    .Schedule(SagaTimeout, context => new SagaTimeoutExpired(context.Saga.OrderId))
                    .TransitionTo(AwaitingStockReservations));

            // ---------------------------------------------------------------
            // 1) Stok rezervasyonu
            // ---------------------------------------------------------------
            During(AwaitingStockReservations,
                When(StockReservedReceived)
                    .Unschedule(StockReservationTimeout)
                    .Publish(context => new ConfirmOrderIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .TransitionTo(AwaitingOrderConfirmation),

                Compensate(When(StockReserveFailedReceived),
                           context => context.Message.FailureReason,
                           releaseStock: false),

                Compensate(When(StockReservationTimeout.Received),
                           _ => "Stock reservation timeout.",
                           releaseStock: true),

                Compensate(When(ReserveStockFaulted),
                           context => FaultReason(context, "Stock reservation failed."),
                           releaseStock: true),

                Compensate(When(SagaTimeout.Received),
                           _ => "Saga timeout.",
                           releaseStock: true));

            // ---------------------------------------------------------------
            // 2) Sipariş onayı
            // ---------------------------------------------------------------
            During(AwaitingOrderConfirmation,
                When(OrderConfirmedReceived)
                    .Publish(context => new ConfirmStockReservationIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .TransitionTo(AwaitingStockConfirmation),

                Compensate(When(ConfirmOrderFaulted),
                           context => FaultReason(context, "Order confirmation failed."),
                           releaseStock: true),

                Compensate(When(SagaTimeout.Received),
                           _ => "Saga timeout while awaiting order confirmation.",
                           releaseStock: true));

            // ---------------------------------------------------------------
            // 3) Stok onayı
            // ---------------------------------------------------------------
            During(AwaitingStockConfirmation,
                When(StockReservationConfirmedReceived)
                    .Publish(context => new CompleteOrderIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .TransitionTo(AwaitingOrderCompletion),

                Compensate(When(StockReservationConfirmFailedReceived),
                           context => context.Message.FailureReason,
                           releaseStock: true),

                Compensate(When(ConfirmStockReservationFaulted),
                           context => FaultReason(context, "Stock reservation confirmation failed."),
                           releaseStock: true),

                Compensate(When(SagaTimeout.Received),
                           _ => "Saga timeout while awaiting stock confirmation.",
                           releaseStock: true));

            // ---------------------------------------------------------------
            // 4) Sipariş tamamlama
            //    Stok bu noktada kesinleşmiş durumda, bu yüzden iptal/release yok.
            //    Hata durumunda saga FailureReason ile kapanır, manuel müdahale gerekir.
            // ---------------------------------------------------------------
            During(AwaitingOrderCompletion,
                When(OrderCompletedReceived)
                    .Finalize(),

                FinalizeWithFailure(When(CompleteOrderFaulted),
                                    context => FaultReason(context, "Order completion failed.")),

                FinalizeWithFailure(When(SagaTimeout.Received),
                                    _ => "Saga timeout while awaiting order completion."),

                // Stok zaten düşülmüş: yeniden stoklama iş kuralı netleşene kadar sadece işaretle.
                FinalizeWithFailure(When(OrderCanceledReceived),
                                    context => $"Order canceled after stock confirmation (manual restock required): {context.Message.Reason}"));

            // ---------------------------------------------------------------
            // 5) İptal bekleme
            // ---------------------------------------------------------------
            During(AwaitingOrderCancellation,
                When(OrderCanceledReceived)
                    .Finalize(),

                // Timeout ile ReserveStock yarıştıysa, geç gelen rezervasyonu geri bırak.
                When(StockReservedReceived)
                    .Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId)),

                FinalizeWithFailure(When(CancelOrderFaulted),
                                    context => FaultReason(context, "Order cancellation failed.")),

                FinalizeWithFailure(When(SagaTimeout.Received),
                                    _ => "Order cancellation timed out."));

            // ---------------------------------------------------------------
            // Kullanıcı kaynaklı iptal (saga sürerken API'den iptal edilirse)
            // ---------------------------------------------------------------
            During(new[] { AwaitingStockReservations, AwaitingOrderConfirmation, AwaitingStockConfirmation },
                When(OrderCanceledReceived)
                    .Then(context => context.Saga.FailureReason ??= $"Order canceled: {context.Message.Reason}")
                    .Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .Finalize());

            // ---------------------------------------------------------------
            // Final: instance silinmiyor, geç gelen event'ler burada karşılanır
            // ---------------------------------------------------------------
            During(Final,
                // Sadece başarısız/iptal edilmiş saga'larda release et (başarılı akışta duplicate gelirse dokunma).
                When(StockReservedReceived)
                    .If(context => context.Saga.FailureReason is not null,
                        x => x.Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))));

            // ---------------------------------------------------------------
            // Geç / duplicate event'ler UnhandledEventException fırlatmasın
            // ---------------------------------------------------------------
            var everyState = new[] { AwaitingStockReservations, AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion, AwaitingOrderCancellation, Final };

            During(everyState,
                Ignore(OrderPlacedReceived));

            During(new[] { AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion },
                Ignore(StockReservedReceived));

            During(new[] { AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion, AwaitingOrderCancellation, Final },
                Ignore(StockReserveFailedReceived),
                Ignore(StockReservationTimeout.Received),
                Ignore(ReserveStockFaulted));

            During(new[] { AwaitingStockConfirmation, AwaitingOrderCompletion, AwaitingOrderCancellation, Final },
                Ignore(OrderConfirmedReceived),
                Ignore(ConfirmOrderFaulted));

            During(new[] { AwaitingOrderCompletion, AwaitingOrderCancellation, Final },
                Ignore(StockReservationConfirmedReceived),
                Ignore(StockReservationConfirmFailedReceived),
                Ignore(ConfirmStockReservationFaulted));

            During(new[] { AwaitingOrderCancellation, Final },
                Ignore(OrderCompletedReceived),
                Ignore(CompleteOrderFaulted));

            During(Final,
                Ignore(OrderCanceledReceived),
                Ignore(CancelOrderFaulted),
                Ignore(SagaTimeout.Received));

            // ---------------------------------------------------------------
            // Final'e girerken: tamamlanma zamanı + bekleyen timeout'ları temizle
            // ---------------------------------------------------------------
            Finally(x => x
                .Then(context => context.Saga.CompletedAtUtc ??= DateTime.UtcNow)
                .Unschedule(StockReservationTimeout)
                .Unschedule(SagaTimeout));

            // SetCompletedWhenFinalized(); // Açılırsa Final instance'ları silinir, geç gelen event'ler (ör. StockReserved) artık yakalanamaz.
        }

        private void ConfigureEvents()
        {
            Event(() => OrderPlacedReceived, config =>
            {
                config.CorrelateById(context => context.Message.OrderId);
                config.SelectId(context => context.Message.OrderId);
            });

            Event(() => StockReservedReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));
            Event(() => StockReserveFailedReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));
            Event(() => StockReservationConfirmedReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));
            Event(() => StockReservationConfirmFailedReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));

            Event(() => OrderConfirmedReceived, config => config.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCompletedReceived, config => config.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCanceledReceived, config => config.CorrelateById(context => context.Message.OrderId));

            Event(() => ReserveStockFaulted, config => config.CorrelateById(context => context.Message.Message.SagaCorrelationId));
            Event(() => ConfirmOrderFaulted, config => config.CorrelateById(context => context.Message.Message.CorrelationId));
            Event(() => ConfirmStockReservationFaulted, config => config.CorrelateById(context => context.Message.Message.SagaCorrelationId));
            Event(() => CompleteOrderFaulted, config => config.CorrelateById(context => context.Message.Message.CorrelationId));
            Event(() => CancelOrderFaulted, config => config.CorrelateById(context => context.Message.Message.CorrelationId));
        }

        private void ConfigureSchedules()
        {
            Schedule(() => StockReservationTimeout, x => x.StockTimeoutTokenId, config =>
            {
                config.Delay = StockReservationTimeoutDelay;
                config.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });

            Schedule(() => SagaTimeout, x => x.SagaTimeoutTokenId, config =>
            {
                config.Delay = SagaTimeoutDelay;
                config.Received = e => e.CorrelateById(context => context.Message.OrderId);
            });
        }

        /// <summary>
        /// FailureReason'ı yazar, (opsiyonel) rezervasyonu serbest bırakır, siparişi iptal ettirir ve iptal bekleme state'ine geçer.
        /// </summary>
        private EventActivityBinder<OrderCheckoutSagaState, T> Compensate<T>(EventActivityBinder<OrderCheckoutSagaState, T> binder,
                                                                             Func<BehaviorContext<OrderCheckoutSagaState, T>, string> reason,
                                                                             bool releaseStock) where T : class
        {
            binder = binder.Then(context => context.Saga.FailureReason = reason(context));

            if (releaseStock)
                binder = binder.Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId));

            return binder
                .Unschedule(StockReservationTimeout)
                .Publish(context => new CancelOrderIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId, context.Saga.FailureReason ?? "Order checkout failed."))
                .TransitionTo(AwaitingOrderCancellation);
        }

        /// <summary>
        /// Telafi edilemeyen durumlarda sebebi kaydedip saga'yı kapatır.
        /// </summary>
        private static EventActivityBinder<OrderCheckoutSagaState, T> FinalizeWithFailure<T>(EventActivityBinder<OrderCheckoutSagaState, T> binder,
                                                                                             Func<BehaviorContext<OrderCheckoutSagaState, T>, string> reason) where T : class
        {
            return binder
                .Then(context => context.Saga.FailureReason ??= reason(context))
                .Finalize();
        }

        private static string FaultReason<T>(BehaviorContext<OrderCheckoutSagaState, Fault<T>> context, string fallback) where T : class
        {
            return context.Message.Exceptions?.FirstOrDefault()?.Message ?? fallback;
        }
    }
}