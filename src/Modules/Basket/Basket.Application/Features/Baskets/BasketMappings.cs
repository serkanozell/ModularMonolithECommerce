namespace Basket.Application.Features.Baskets
{
    public static class BasketMappings
    {
        public static BasketDto ToDto(this ShoppingCart cart) =>
            new(cart.Id,
                cart.UserName,
                cart.Items.Select(i => i.ToDto())
                          .ToList());

        public static BasketItemDto ToDto(this ShoppingCartItem item) =>
            new(
                item.Id,
                item.ShoppingCartId,
                item.ProductId,
                item.ProductName,
                item.Color,
                item.Quantity,
                item.Price);
    }
}
