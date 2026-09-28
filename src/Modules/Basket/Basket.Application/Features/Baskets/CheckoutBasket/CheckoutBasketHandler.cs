using Basket.Application.Features.Baskets.Dtos;
using Basket.Domain.Exceptions;
using BuildingBlocks.Messaging.Events;
using BuildingBlocks.Shared.Dtos;
using MassTransit;

namespace Basket.Application.Features.Baskets.CheckoutBasket
{
    public record CheckoutBasketCommand(BasketCheckoutDto BasketCheckout)
        : ICommand<CheckoutBasketResult>;
    public record CheckoutBasketResult(bool IsSuccess);
    public class CheckoutBasketCommandValidator : AbstractValidator<CheckoutBasketCommand>
    {
        public CheckoutBasketCommandValidator()
        {
            RuleFor(x => x.BasketCheckout).NotNull().WithMessage("BasketCheckoutDto can't be null");
            RuleFor(x => x.BasketCheckout.UserName).NotEmpty().WithMessage("UserName is required");
        }
    }

    // Transaction, outbox persistence and commit are handled by TransactionBehavior
    internal sealed class CheckoutBasketHandler(IBasketRepository basketRepository, IPublishEndpoint publishEndpoint, ISender sender) : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            var checkout = command.BasketCheckout;

            var basket = await basketRepository.GetBasket(checkout.UserName, cancellationToken: cancellationToken)
                ?? throw new BasketNotFoundException(checkout.UserName);

            // Prices are re-validated against catalog so that the order is always created with current prices
            foreach (var productId in basket.Items.Select(i => i.ProductId).Distinct().ToList())
            {
                var result = await sender.Send(new GetProductByIdQuery(productId), cancellationToken);

                basket.UpdateItemPrice(productId, result.Product.Price, DateTime.UtcNow);
            }

            // kontrol sonrası totalprice değiştiyse kullanıcıya bilgi verilmeli.
            // tüm id leri distinct alıp hepsini tek seferde getiren servis yapılabilir.

            var eventMessage = new BasketCheckoutIntegrationEvent
            {
                UserName = checkout.UserName,
                CustomerId = checkout.CustomerId,
                TotalPrice = basket.TotalPrice,
                FirstName = checkout.FirstName,
                LastName = checkout.LastName,
                EmailAddress = checkout.EmailAddress,
                AddressLine = checkout.AddressLine,
                Country = checkout.Country,
                State = checkout.State,
                ZipCode = checkout.ZipCode,
                CardName = checkout.CardName,
                CardNumber = checkout.CardNumber,
                Expiration = checkout.Expiration,
                Cvv = checkout.Cvv,
                PaymentMethod = checkout.PaymentMethod,
                OrderItems = basket.Items.Select(i => new OrderItemDto(i.ProductId, i.Quantity, i.Price)).ToList()
            };

            // Written to messaging.OutboxMessage via MassTransit bus outbox
            await publishEndpoint.Publish(eventMessage, cancellationToken);

            await basketRepository.DeleteBasket(checkout.UserName, cancellationToken);

            return new CheckoutBasketResult(true);
        }
    }
}