namespace Ordering.Domain.Entities
{
    public class OrderItem : Entity<OrderItemId>
    {
        private OrderItem() { }

        private OrderItem(OrderId orderId, ProductId productId, Quantity quantity, Price price)
        {
            Id = OrderItemId.Of(Guid.NewGuid());
            OrderId = orderId;
            ProductId = productId;
            Quantity = quantity;
            Price = price;
            IsActive = true;
            IsDeleted = false;
        }

        public OrderId OrderId { get; private set; } = default!;
        public ProductId ProductId { get; private set; } = default!;
        public Quantity Quantity { get; private set; } = default!;
        public Price Price { get; private set; } = default!;

        internal static OrderItem Create(OrderId orderId, ProductId productId, Quantity quantity, Price price)
        {
            ArgumentNullException.ThrowIfNull(orderId);
            ArgumentNullException.ThrowIfNull(productId);
            ArgumentNullException.ThrowIfNull(quantity);
            ArgumentNullException.ThrowIfNull(price);

            return new OrderItem(orderId, productId, quantity, price);
        }

        internal void IncreaseQuantity(Quantity quantity)
        {
            Quantity = Quantity.Increase(quantity);
        }
    }
}
