using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Shared.Dtos;
using MassTransit;
using Microsoft.Extensions.Logging;
using Ordering.Application.Features.Orders.CreateOrder;

namespace Ordering.Application.Features.Orders.EventHandlers
{
    public sealed class BasketCheckoutIntegrationEventHandler(ISender sender, ILogger<BasketCheckoutIntegrationEventHandler> logger) : IConsumer<BasketCheckoutIntegrationEvent>
    {
        public async Task Consume(ConsumeContext<BasketCheckoutIntegrationEvent> context)
        {
            logger.LogInformation("BasketCheckoutIntegrationEventHandler received a message: {Message}", context.Message);

            var address = new AddressDto(
                context.Message.FirstName,
                context.Message.LastName,
                context.Message.EmailAddress,
                context.Message.AddressLine,
                context.Message.Country,
                context.Message.State,
                context.Message.ZipCode
            );

            var payment = new PaymentDto(
                context.Message.CardName,
                context.Message.CardNumber,
                context.Message.Expiration,
                context.Message.Cvv,
                context.Message.PaymentMethod
            );

            var orderItems = context.Message.OrderItems.Select(item => new OrderItemDto(
                item.ProductId,
                item.Quantity,
                item.Price
            )).ToList();

            var command = new CreateOrderCommand(context.Message.CustomerId,
                                                 address,
                                                 address,
                                                 payment,
                                                 orderItems);

            await sender.Send(command, context.CancellationToken);
        }
    }
}
