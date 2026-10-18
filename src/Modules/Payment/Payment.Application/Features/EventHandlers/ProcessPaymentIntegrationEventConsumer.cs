using MassTransit;
using MediatR;
using Payment.Application.Features.ProcessPayment;
using Payment.Contracts.PaymentIntegrationEvents;

namespace Payment.Application.Features.EventHandlers
{
    public sealed class ProcessPaymentIntegrationEventConsumer(ISender sender) : IConsumer<ProcessPaymentIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<ProcessPaymentIntegrationEvent> context)
        {
            var message = context.Message;
            var result = await sender.Send(new ProcessPaymentCommand(message.OrderId, message.Amount), context.CancellationToken);

            if (result.IsSuccess)
                await context.Publish(new PaymentSucceededIntegrationEvent(message.SagaCorrelationId, message.OrderId), context.CancellationToken);
            else
                await context.Publish(new PaymentFailedIntegrationEvent(message.SagaCorrelationId, message.OrderId, result.FailureReason ?? "Payment failed."), context.CancellationToken);
        }
    }
}