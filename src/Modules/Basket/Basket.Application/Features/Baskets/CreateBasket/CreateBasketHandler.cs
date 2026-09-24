using Basket.Application.Features.Baskets.Dtos;

namespace Basket.Application.Features.Baskets.CreateBasket
{
    public record CreateBasketCommand(CreateBasketDto CreateBasketDto) : ICommand<CreateBasketResult>;

    public record CreateBasketResult(Guid Id);

    public class CreateBasketCommandValidator : AbstractValidator<CreateBasketCommand>
    {
        public CreateBasketCommandValidator()
        {
            RuleFor(x => x.CreateBasketDto.UserName)
                .NotEmpty()
                .WithMessage("UserName is required.")
                .MaximumLength(100)
                .WithMessage("UserName must not exceed 100 characters.");
        }
    }

    public class CreateBasketCommandHandler(IBasketRepository repository, ISender sender) : ICommandHandler<CreateBasketCommand, CreateBasketResult>
    {
        public async Task<CreateBasketResult> Handle(CreateBasketCommand command, CancellationToken cancellationToken)
        {
            var basket = await CreateNewBasket(command.CreateBasketDto, cancellationToken);

            await repository.CreateBasket(basket, cancellationToken);

            return new CreateBasketResult(basket.Id);
        }

        private async Task<ShoppingCart> CreateNewBasket(CreateBasketDto createBasketDto, CancellationToken cancellationToken)
        {
            // create new basket
            var newBasket = ShoppingCart.Create(createBasketDto.UserName);

            foreach (var item in createBasketDto.Items)
            {
                // price and name are always resolved from catalog, never trusted from client input
                var result = await sender.Send(new GetProductByIdQuery(item.ProductId), cancellationToken);

                newBasket.AddItem(
                    item.ProductId,
                    item.Quantity,
                    item.Color,
                    result.Product.Price,
                    result.Product.Name);
            }

            return newBasket;
        }
    }
}