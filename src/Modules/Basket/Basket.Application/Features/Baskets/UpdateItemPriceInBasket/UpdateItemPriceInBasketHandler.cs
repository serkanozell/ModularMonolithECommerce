namespace Basket.Application.Features.Baskets.UpdateItemPriceInBasket
{
    public record UpdateItemPriceInBasketCommand(Guid ProductId, decimal Price, DateTime PriceChangedAtUtc) : ICommand<UpdateItemPriceInBasketResult>;
    public record UpdateItemPriceInBasketResult(bool IsSuccess);
    public class UpdateItemPriceInBasketCommandValidator : AbstractValidator<UpdateItemPriceInBasketCommand>
    {
        public UpdateItemPriceInBasketCommandValidator()
        {
            RuleFor(x => x.ProductId).NotEmpty().WithMessage("ProductId is required");
            RuleFor(x => x.Price).GreaterThan(0).WithMessage("Price must be greater than 0");
            RuleFor(x => x.PriceChangedAtUtc).NotEmpty().WithMessage("PriceChangedAtUtc is required");
        }
    }

    internal sealed class UpdateItemPriceInBasketHandler(IBasketRepository basketRepository) : ICommandHandler<UpdateItemPriceInBasketCommand, UpdateItemPriceInBasketResult>
    {
        public async Task<UpdateItemPriceInBasketResult> Handle(UpdateItemPriceInBasketCommand command, CancellationToken cancellationToken)
        {
            var itemsToUpdate = await basketRepository.GetBasketItemsByProductId(command.ProductId, cancellationToken);

            if (itemsToUpdate == null || !itemsToUpdate.Any())
                return new UpdateItemPriceInBasketResult(false);

            foreach (var item in itemsToUpdate)
            {
                item.UpdatePrice(command.Price, command.PriceChangedAtUtc);
            }

            await basketRepository.SaveChangesAsync(cancellationToken: cancellationToken);

            return new UpdateItemPriceInBasketResult(true);
        }
    }
}