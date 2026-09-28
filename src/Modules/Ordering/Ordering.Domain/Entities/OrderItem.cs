namespace Ordering.Domain.Entities
{
    public class OrderItem : Entity<Guid>
    {
        private OrderItem() { }

        private OrderItem(Guid orderId, Guid productId, int quantity, decimal price)
        {
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            IsActive = true;
            IsDeleted = false;
        }

        public Guid OrderId { get; private set; } = default!;
        public Guid ProductId { get; private set; } = default!;
        public int Quantity { get; private set; } = default!;
        public decimal Price { get; private set; } = default!;

        internal static OrderItem Create(Guid orderId, Guid productId, int quantity, decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(productId, Guid.Empty);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            return new OrderItem(orderId, productId, quantity, price);
        }

        internal void IncreaseQuantity(int quantity)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            Quantity += quantity;
        }
    }
}
