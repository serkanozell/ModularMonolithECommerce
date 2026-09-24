using System.Text.Json.Serialization;

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
        public DateTime PriceUpdatedAtUtc { get; private set; }

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
            PriceUpdatedAtUtc = DateTime.UtcNow;
            IsActive = true;
            IsDeleted = false;
        }

        [JsonConstructor]
        public ShoppingCartItem(Guid id, Guid shoppingCartId, Guid productId, int quantity, string color, decimal price, string productName)
        {
            Id = id;
            ShoppingCartId = shoppingCartId;
            ProductId = productId;
            Quantity = quantity;
            Color = color;
            Price = price;
            ProductName = productName;
            PriceUpdatedAtUtc = DateTime.UtcNow;
            IsActive = true;
            IsDeleted = false;
        }

        public bool UpdatePrice(decimal newPrice, DateTime priceChangedAtUtc)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(newPrice);

            if (priceChangedAtUtc <= PriceUpdatedAtUtc)
                return false;

            Price = newPrice;
            PriceUpdatedAtUtc = priceChangedAtUtc;
            return true;
        }
    }
}