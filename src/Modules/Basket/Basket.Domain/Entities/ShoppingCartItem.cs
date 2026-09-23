namespace Basket.Domain.Entities
{
    public class ShoppingCartItem : Entity<Guid>
    {
        public Guid ShoppingCartId { get; private set; } = default!;
        public Guid ProductId { get; private set; } = default!;
        public int Quantity { get; internal set; } = default!;
        public string Color { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;
        public string ProductName { get; private set; } = default!;

        private ShoppingCartItem()
        {
        }

        internal ShoppingCartItem(Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
        {
            Id = Guid.NewGuid();
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
            Color = color;
            Price = price;
            ProductName = productName;
            IsActive = true;
            IsDeleted = false;
        }
    }
}