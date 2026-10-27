using MassTransit;
using MediatR;
using Payment.Application.Features.RefundPayment;
using Payment.Contracts.PaymentIntegrationEvents;

namespace Payment.Application.Features.EventHandlers
{
    public sealed class RefundPaymentIntegrationEventConsumer(ISender sender) : IConsumer<RefundPaymentIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<RefundPaymentIntegrationEvent> context)
        {
            var result = await sender.Send(new RefundPaymentCommand(context.Message.OrderId), context.CancellationToken);

            await context.Publish(new PaymentRefundResultIntegrationEvent(
                context.Message.SagaCorrelationId, context.Message.OrderId, result.IsSuccess,
                result.IsSuccess ? null : "Payment refund failed."), context.CancellationToken);
        }
    }
}