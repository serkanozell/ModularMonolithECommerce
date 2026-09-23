using Basket.Domain.Exceptions;

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

    internal class CheckoutBasketHandler(IBasketRepository dbContext)
        : ICommandHandler<CheckoutBasketCommand, CheckoutBasketResult>
    {
        public async Task<CheckoutBasketResult> Handle(CheckoutBasketCommand command, CancellationToken cancellationToken)
        {
            // get existing basket with total price
            // Set totalprice on basketcheckout event message
            // send basket checkout event to rabbitmq using masstransit
            // delete the basket

            //await using var transaction =
            //    await dbContext.Database.BeginTransactionAsync(cancellationToken);

            // outbox + Masstransit eklenmeli
            try
            {
                // Get existing basket with total price
                //var basket = await dbContext.ShoppingCarts
                //    .Include(x => x.Items)
                //    .SingleOrDefaultAsync(x => x.UserName == command.BasketCheckout.UserName, cancellationToken);

                //if (basket == null)
                //{
                //    throw new BasketNotFoundException(command.BasketCheckout.UserName);
                //}

                //// Set total price on basket checkout event message
                //var eventMessage = command.BasketCheckout.Adapt<BasketCheckoutIntegrationEvent>();
                //eventMessage.TotalPrice = basket.TotalPrice;

                //// Write a message to the outbox
                //var outboxMessage = new OutboxMessage
                //{
                //    Id = Guid.NewGuid(),
                //    Type = typeof(BasketCheckoutIntegrationEvent).AssemblyQualifiedName!,
                //    Content = JsonSerializer.Serialize(eventMessage),
                //    OccuredOn = DateTime.UtcNow
                //};

                //dbContext.OutboxMessages.Add(outboxMessage);

                //// Delete the basket
                //dbContext.ShoppingCarts.Remove(basket);

                //await dbContext.SaveChangesAsync(cancellationToken);
                //await transaction.CommitAsync(cancellationToken);

                return new CheckoutBasketResult(true);
            }
            catch
            {
                //await transaction.RollbackAsync(cancellationToken);
                return new CheckoutBasketResult(false);
            }
        }
    }
}