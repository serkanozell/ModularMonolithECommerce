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
            var baskets = await basketRepository.GetBasketsByProductId(command.ProductId, cancellationToken);

            var updatedBaskets = baskets
                .Where(basket => basket.UpdateItemPrice(command.ProductId, command.Price, command.PriceChangedAtUtc))
                .ToList();

            if (updatedBaskets.Count == 0)
                return new UpdateItemPriceInBasketResult(false);

            await basketRepository.UpdateBaskets(updatedBaskets, cancellationToken);

            return new UpdateItemPriceInBasketResult(true);
        }
    }

    ///
    /// ürün fiyatı değişti
    /// event fırlatıldı
    /// basketteki UpdateItemPriceInBasketHandler a geldi
    /// fiyatı değişen ürünün olduğu tüm sepetler dbden çekilir. çünkü aggregate üzerinden işlem yapmalıyız
    /// tüm sepetlerdeki ilgili ürünün fiyatı güncellenir.
    /// tüm sepetler güncellenir ve db ye kaydedilir.
    /// update yapıldığı için cache deki sepetler silinir çünkü cache deki sepetler artık eski fiyatı gösteriyor.
    /// 
    ///

}
