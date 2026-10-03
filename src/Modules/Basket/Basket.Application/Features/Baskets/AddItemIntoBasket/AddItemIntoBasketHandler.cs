using Basket.Application.Features.Baskets.Dtos;

namespace Basket.Application.Features.Baskets.AddItemIntoBasket
{
    public record AddItemIntoBasketCommand(string UserName, BasketItemDto BasketItemDto) : ICommand<AddItemIntoBasketResult>;
    public record AddItemIntoBasketResult(Guid Id);
    public class AddItemIntoBasketCommandValidator : AbstractValidator<AddItemIntoBasketCommand>
    {
        public AddItemIntoBasketCommandValidator()
        {
            RuleFor(x => x.UserName).NotEmpty().WithMessage("UserName is required");
            RuleFor(x => x.BasketItemDto.ProductId).NotEmpty().WithMessage("ProductId is required");
            RuleFor(x => x.BasketItemDto.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than 0");
        }
    }

    internal sealed class AddItemIntoBasketHandler(IBasketRepository repository, ISender sender) : ICommandHandler<AddItemIntoBasketCommand, AddItemIntoBasketResult>
    {
        public async Task<AddItemIntoBasketResult> Handle(AddItemIntoBasketCommand command, CancellationToken cancellationToken)
        {
            // Add shopping cart item into shopping cart
            var shoppingCart = await repository.GetBasket(command.UserName, false, cancellationToken);

            ///
            /// bu feature ileride catalog.contracts üzerinden çalışacak servis mantığına çevrilebilir.
            /// catalog.application katmanı catalog.contracts a referans verdiği için doğru bir yaklaşım olmayabilir.
            /// bunun yerine contracts içinde bir getproductbyid servisi ile kullanılabilir.
            ///
            var result = await sender.Send(new GetProductByIdQuery(command.BasketItemDto.ProductId), cancellationToken);

            shoppingCart.AddItem(command.BasketItemDto.ProductId,
                                 command.BasketItemDto.Quantity,
                                 command.BasketItemDto.Color,
                                 result.Product.Price,
                                 result.Product.Name);

            await repository.SaveChangesAsync(command.UserName, cancellationToken);

            return new AddItemIntoBasketResult(shoppingCart.Id);
        }
    }
}