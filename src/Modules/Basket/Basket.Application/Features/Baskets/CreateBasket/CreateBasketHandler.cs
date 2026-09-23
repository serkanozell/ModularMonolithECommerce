namespace Basket.Application.Features.Baskets.CreateBasket
{
    public record CreateBasketCommand(BasketDto BasketDto) : ICommand<CreateBasketResult>;

    public record CreateBasketResult(Guid Id);

    public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
    {
        public CreateBasketCommandValidator()
        {
            RuleFor(x => x.BasketDto.UserName)
                .NotEmpty()
                .WithMessage("UserName is required.")
                .MaximumLength(100)
                .WithMessage("UserName must not exceed 100 characters.");
        }
    }

    public class CreateBasketCommandHandler(IBasketRepository repository) : ICommandHandler<CreateBasketCommand, CreateBasketResult>
    {
        public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = CreateNewBasket(command.BasketDto);

            await repository.CreateBasket(basket, cancellationToken);

            return new CreateBasketResult(basket.Id);
        }

        private static ShoppingCart CreateNewBasket(BasketDto basketDto)
        {
            // create new basket
            var newBasket = ShoppingCart.Create(basketDto.UserName);

            basketDto.Items.ForEach(item =>
            {
                newBasket.AddItem(
                    item.ProductId,
                    item.Quantity,
                    item.Color,
                    item.Price,
                    item.ProductName);
            });

            return newBasket;
        }
    }
}