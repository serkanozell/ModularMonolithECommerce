using BuildingBlocks.Messaging.Events;
using Inventory.Contracts;
using MassTransit;
using Payment.Contracts.PaymentIntegrationEvents;

namespace Ordering.Application.Features.Orders.Sagas
{
    public sealed class OrderCheckoutStateMachine : MassTransitStateMachine<OrderCheckoutSagaState>
    {
        private static readonly TimeSpan StockReservationTimeoutDelay = TimeSpan.FromSeconds(30);
        private static readonly TimeSpan SagaTimeoutDelay = TimeSpan.FromMinutes(5);

        // States
        public State AwaitingStockReservations { get; private set; } = null!;
        public State AwaitingPayment { get; private set; } = null!;
        public State AwaitingOrderConfirmation { get; private set; } = null!;
        public State AwaitingStockConfirmation { get; private set; } = null!;
        public State AwaitingOrderCompletion { get; private set; } = null!;
        public State AwaitingOrderCancellation { get; private set; } = null!;

        // Events
        public Event<OrderPlacedIntegrationEvent> OrderPlacedReceived { get; private set; } = null!;
        public Event<StockReservedIntegrationEvent> StockReservedReceived { get; private set; } = null!;
        public Event<StockReserveFailedIntegrationEvent> StockReserveFailedReceived { get; private set; } = null!;
        public Event<PaymentSucceededIntegrationEvent> PaymentSucceededReceived { get; private set; } = null!;
        public Event<PaymentFailedIntegrationEvent> PaymentFailedReceived { get; private set; } = null!;
        public Event<PaymentRefundResultIntegrationEvent> PaymentRefundResultReceived { get; private set; } = null!;
        public Event<OrderConfirmedIntegrationEvent> OrderConfirmedReceived { get; private set; } = null!;
        public Event<StockReservationConfirmedIntegrationEvent> StockReservationConfirmedReceived { get; private set; } = null!;
        public Event<StockReservationConfirmFailedIntegrationEvent> StockReservationConfirmFailedReceived { get; private set; } = null!;
        public Event<StockReservationReleaseResultIntegrationEvent> StockReservationReleaseResultReceived { get; private set; } = null!;
        public Event<OrderCompletedIntegrationEvent> OrderCompletedReceived { get; private set; } = null!;
        public Event<OrderCanceledIntegrationEvent> OrderCanceledReceived { get; private set; } = null!;

        // Fault events (consumer retry'ları tükenince MassTransit Fault<T> yayınlar)
        public Event<Fault<ReserveStockIntegrationEvent>> ReserveStockFaulted { get; private set; } = null!;
        public Event<Fault<ReleaseReserveIntegrationEvent>> ReleaseReserveFaulted { get; private set; } = null!;
        public Event<Fault<ProcessPaymentIntegrationEvent>> ProcessPaymentFaulted { get; private set; } = null!;
        public Event<Fault<RefundPaymentIntegrationEvent>> RefundPaymentFaulted { get; private set; } = null!;
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
                    .Publish(context => new ProcessPaymentIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId, context.Saga.CustomerId, context.Saga.TotalAmount))
                    .TransitionTo(AwaitingPayment),

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
            // 2) Ödeme
            //    Başarısız ödemede sadece stok serbest bırakılır (para alınmadı).
            //    Fault / timeout durumunda ödeme belirsiz olduğu için iade de istenir
            //    (Payment tarafında iade idempotent: kayıt yoksa / Succeeded değilse no-op).
            //    İade, ödeme kaydından önce işlenirse geç gelen PaymentSucceeded
            //    AwaitingOrderCancellation / Final state'inde tekrar iade tetikler.
            // ---------------------------------------------------------------
            During(AwaitingPayment,
                When(PaymentSucceededReceived)
                    .Publish(context => new ConfirmOrderIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .TransitionTo(AwaitingOrderConfirmation),

                Compensate(When(PaymentFailedReceived),
                           context => context.Message.FailureReason,
                           releaseStock: true),

                Compensate(When(ProcessPaymentFaulted),
                           context => FaultReason(context, "Payment processing failed."),
                           releaseStock: true, refundPayment: true),

                Compensate(When(SagaTimeout.Received),
                           _ => "Saga timeout while awaiting payment.",
                           releaseStock: true, refundPayment: true));

            // ---------------------------------------------------------------
            // 3) Sipariş onayı
            // ---------------------------------------------------------------
            During(AwaitingOrderConfirmation,
                When(OrderConfirmedReceived)
                    .Publish(context => new ConfirmStockReservationIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .TransitionTo(AwaitingStockConfirmation),

                Compensate(When(ConfirmOrderFaulted),
                           context => FaultReason(context, "Order confirmation failed."),
                           releaseStock: true,
                           refundPayment: true),

                Compensate(When(SagaTimeout.Received),
                           _ => "Saga timeout while awaiting order confirmation.",
                           releaseStock: true,
                           refundPayment: true));

            // ---------------------------------------------------------------
            // 4) Stok onayı
            // ---------------------------------------------------------------
            During(AwaitingStockConfirmation,
                When(StockReservationConfirmedReceived)
                    .Publish(context => new CompleteOrderIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .TransitionTo(AwaitingOrderCompletion),

                Compensate(When(StockReservationConfirmFailedReceived),
                           context => context.Message.FailureReason,
                           releaseStock: true,
                           refundPayment: true),

                Compensate(When(ConfirmStockReservationFaulted),
                           context => FaultReason(context, "Stock reservation confirmation failed."),
                           releaseStock: true,
                           refundPayment: true),

                Compensate(When(SagaTimeout.Received),
                           _ => "Saga timeout while awaiting stock confirmation.",
                           releaseStock: true,
                           refundPayment: true));

            // ---------------------------------------------------------------
            // 5) Sipariş tamamlama
            //    Stok bu noktada kesinleşmiş durumda, bu yüzden stok release yok
            //    (manuel restock gerekir). Ödeme ise alınmış durumda olduğu için
            //    sipariş iptal edilirse iade yayınlanır.
            //    Complete hatası / timeout'ta saga FailureReason ile kapanır, manuel müdahale gerekir.
            // ---------------------------------------------------------------
            During(AwaitingOrderCompletion,
                When(OrderCompletedReceived)
                    .Finalize(),

                FinalizeWithFailure(When(CompleteOrderFaulted),
                                    context => FaultReason(context, "Order completion failed.")),

                FinalizeWithFailure(When(SagaTimeout.Received),
                                    _ => "Saga timeout while awaiting order completion."),

                When(OrderCanceledReceived)
                    .Then(context => context.Saga.FailureReason ??= $"Order canceled after stock confirmation (manual restock required): {context.Message.Reason}")
                    .Publish(context => new RefundPaymentIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .Finalize());

            // ---------------------------------------------------------------
            // 6) İptal bekleme
            // ---------------------------------------------------------------
            During(AwaitingOrderCancellation,
                When(OrderCanceledReceived)
                    .Finalize(),

                // Timeout ile ReserveStock yarıştıysa, geç gelen rezervasyonu geri bırak.
                When(StockReservedReceived)
                    .Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId)),

                // Telafi sürerken ödeme sonradan başarılı olduysa iade et.
                LatePaymentSucceeded(),
                OnPaymentRefundResult(),
                OnRefundPaymentFaulted(),
                OnStockReleaseResult(),
                OnReleaseReserveFaulted(),

                FinalizeWithFailure(When(CancelOrderFaulted),
                                    context => FaultReason(context, "Order cancellation failed.")),

                FinalizeWithFailure(When(SagaTimeout.Received),
                                    _ => "Order cancellation timed out."));

            // ---------------------------------------------------------------
            // Kullanıcı kaynaklı iptal (saga sürerken API'den iptal edilirse)
            // ---------------------------------------------------------------

            // Henüz ödeme istenmedi: sadece stok serbest bırakılır.
            During(AwaitingStockReservations,
                When(OrderCanceledReceived)
                    .Then(context => context.Saga.FailureReason ??= $"Order canceled: {context.Message.Reason}")
                    .Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .Finalize());

            // Ödeme istenmiş / alınmış olabilir: stok + iade (iade idempotent).
            // AwaitingPayment'ta ödeme uçuşta olabilir; iade ödemeden önce işlenirse
            // Final'deki geç PaymentSucceeded dalı tekrar iade tetikler.
            During(new[] { AwaitingPayment, AwaitingOrderConfirmation, AwaitingStockConfirmation },
                When(OrderCanceledReceived)
                    .Then(context => context.Saga.FailureReason ??= $"Order canceled: {context.Message.Reason}")
                    .Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .Publish(context => new RefundPaymentIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))
                    .Finalize());

            // ---------------------------------------------------------------
            // Final: instance silinmiyor, geç gelen event'ler burada karşılanır
            // ---------------------------------------------------------------
            During(Final,
                // Sadece başarısız/iptal edilmiş saga'larda release et (başarılı akışta duplicate gelirse dokunma).
                When(StockReservedReceived)
                    .If(context => context.Saga.FailureReason is not null,
                        x => x.Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId))),

                // Sadece başarısız/iptal edilmiş saga'larda iade et (başarılı akışta duplicate gelirse dokunma).
                LatePaymentSucceeded(),
                OnPaymentRefundResult(),
                OnRefundPaymentFaulted(),
                OnStockReleaseResult(),
                OnReleaseReserveFaulted());

            // ---------------------------------------------------------------
            // Geç / duplicate event'ler UnhandledEventException fırlatmasın
            // ---------------------------------------------------------------
            var everyState = new[] { AwaitingStockReservations, AwaitingPayment, AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion, AwaitingOrderCancellation, Final };

            During(everyState,
                Ignore(OrderPlacedReceived));

            During(new[] { AwaitingPayment, AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion },
                Ignore(StockReservedReceived));

            During(new[] { AwaitingPayment, AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion, AwaitingOrderCancellation, Final },
                Ignore(StockReserveFailedReceived),
                Ignore(StockReservationTimeout.Received),
                Ignore(ReserveStockFaulted));

            // Ödeme sonucu: AwaitingPayment dışındaki state'lerde duplicate / geç gelen mesajlar.
            // PaymentSucceeded; AwaitingOrderCancellation ve Final'de LatePaymentSucceeded ile ele alınıyor.
            During(new[] { AwaitingStockReservations, AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion },
                Ignore(PaymentSucceededReceived));

            During(new[] { AwaitingStockReservations, AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion, AwaitingOrderCancellation, Final },
                Ignore(PaymentFailedReceived),
                Ignore(ProcessPaymentFaulted));

            // İade sonucu / fault: AwaitingOrderCancellation ve Final'de işleniyor,
            // diğer state'lerde (ör. kullanıcı iptalinden sonra erken gelen duplicate) yok sayılır.
            During(new[] { AwaitingStockReservations, AwaitingPayment, AwaitingOrderConfirmation, AwaitingStockConfirmation, AwaitingOrderCompletion },
                Ignore(PaymentRefundResultReceived),
                Ignore(RefundPaymentFaulted));

            During(new[] { AwaitingPayment, AwaitingStockConfirmation, AwaitingOrderCompletion, AwaitingOrderCancellation, Final },
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

            // SetCompletedWhenFinalized(); // Açılırsa Final instance'ları silinir, geç gelen event'ler (ör. StockReserved, PaymentSucceeded) artık yakalanamaz.
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
            Event(() => StockReservationReleaseResultReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));

            Event(() => PaymentSucceededReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));
            Event(() => PaymentFailedReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));
            Event(() => PaymentRefundResultReceived, config => config.CorrelateById(context => context.Message.SagaCorrelationId));

            Event(() => OrderConfirmedReceived, config => config.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCompletedReceived, config => config.CorrelateById(context => context.Message.OrderId));
            Event(() => OrderCanceledReceived, config => config.CorrelateById(context => context.Message.OrderId));

            Event(() => ReserveStockFaulted, config => config.CorrelateById(context => context.Message.Message.SagaCorrelationId));
            Event(() => ReleaseReserveFaulted, config => config.CorrelateById(context => context.Message.Message.SagaCorrelationId));
            Event(() => ProcessPaymentFaulted, config => config.CorrelateById(context => context.Message.Message.SagaCorrelationId));
            Event(() => RefundPaymentFaulted, config => config.CorrelateById(context => context.Message.Message.SagaCorrelationId));
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

        // ---------------------------------------------------------------
        // AwaitingOrderCancellation ve Final için ortak "geç gelen event" dalları
        // ---------------------------------------------------------------

        /// <summary>
        /// Saga başarısız/iptal olduktan sonra ödeme başarılı olduysa (iade, ödeme kaydından önce işlendiyse
        /// no-op olduğu için) parayı tekrar iade ettirir. Başarılı akışta FailureReason null olduğundan dokunmaz.
        /// </summary>
        private EventActivityBinder<OrderCheckoutSagaState, PaymentSucceededIntegrationEvent> LatePaymentSucceeded() =>
            When(PaymentSucceededReceived)
                .If(context => context.Saga.FailureReason is not null,
                    x => x.Publish(context => new RefundPaymentIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId)));

        private EventActivityBinder<OrderCheckoutSagaState, PaymentRefundResultIntegrationEvent> OnPaymentRefundResult() =>
            When(PaymentRefundResultReceived)
                .Then(context =>
                {
                    if (!context.Message.IsSuccess)
                        RecordFailure(context.Saga, "Payment refund failed", context.Message.FailureReason);
                });

        private EventActivityBinder<OrderCheckoutSagaState, Fault<RefundPaymentIntegrationEvent>> OnRefundPaymentFaulted() =>
            When(RefundPaymentFaulted)
                .Then(context => RecordFailure(context.Saga, "Payment refund faulted", FaultReason(context, "Payment refund faulted.")));

        private EventActivityBinder<OrderCheckoutSagaState, StockReservationReleaseResultIntegrationEvent> OnStockReleaseResult() =>
            When(StockReservationReleaseResultReceived)
                .Then(context =>
                {
                    if (!context.Message.IsSuccess)
                        RecordFailure(context.Saga, "Stock reservation release failed", context.Message.FailureReason);
                });

        private EventActivityBinder<OrderCheckoutSagaState, Fault<ReleaseReserveIntegrationEvent>> OnReleaseReserveFaulted() =>
            When(ReleaseReserveFaulted)
                .Then(context => RecordFailure(context.Saga, "Stock reservation release faulted", FaultReason(context, "Stock reservation release faulted.")));

        /// <summary>
        /// FailureReason'ı yazar, (opsiyonel) rezervasyonu serbest bırakır, (opsiyonel) ödemeyi iade ettirir,
        /// siparişi iptal ettirir ve iptal bekleme state'ine geçer.
        /// </summary>
        private EventActivityBinder<OrderCheckoutSagaState, T> Compensate<T>(EventActivityBinder<OrderCheckoutSagaState, T> binder,
                                                                             Func<BehaviorContext<OrderCheckoutSagaState, T>, string> reason,
                                                                             bool releaseStock,
                                                                             bool refundPayment = false) where T : class
        {
            binder = binder.Then(context => context.Saga.FailureReason = reason(context));

            if (releaseStock)
                binder = binder.Publish(context => new ReleaseReserveIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId));

            if (refundPayment)
                binder = binder.Publish(context => new RefundPaymentIntegrationEvent(context.Saga.CorrelationId, context.Saga.OrderId));

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

        private static void RecordFailure(OrderCheckoutSagaState saga, string prefix, string? reason)
        {
            var failure = string.IsNullOrWhiteSpace(reason) ? $"{prefix}." : reason;
            if (saga.FailureReason?.Contains(failure, StringComparison.OrdinalIgnoreCase) == true) return;

            saga.FailureReason = string.IsNullOrWhiteSpace(saga.FailureReason) ? $"{prefix}: {failure}" : $"{saga.FailureReason} {prefix}: {failure}";
        }
    }
}