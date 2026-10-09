using Ordering.Domain.Enums;

namespace Ordering.Domain.Entities
{
    public class Order : Aggregate<OrderId>
    {
        private readonly List<OrderItem> _items = new();
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        public CustomerId CustomerId { get; private set; } = default!;
        public OrderName OrderName { get; private set; } = default!;
        public OrderStatus OrderStatus { get; private set; }
        public Address ShippingAddress { get; private set; } = default!;
        public Address BillingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public decimal TotalPrice => Items.Sum(x => x.Price.Value * x.Quantity.Value);


        private Order() { }

        private Order(CustomerId customerId, Address shippingAddress, Address billingAddress, Payment payment)
        {
            Id = OrderId.Of(Guid.NewGuid());
            CustomerId = customerId;
            OrderName = OrderName.Of($"ORD-" + $"{DateTime.UtcNow:yyyyMMdd-HHmmss}-{Guid.NewGuid().ToString()[..8].ToUpperInvariant()}");
            OrderStatus = OrderStatus.Pending;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            Payment = payment;
            IsActive = true;
            IsDeleted = false;
        }

        public static Order Create(CustomerId customerId, Address shippingAddress, Address billingAddress, Payment payment)
        {
            ArgumentNullException.ThrowIfNull(customerId);
            ArgumentNullException.ThrowIfNull(shippingAddress);
            ArgumentNullException.ThrowIfNull(billingAddress);
            ArgumentNullException.ThrowIfNull(payment);

            var order = new Order(customerId, shippingAddress, billingAddress, payment);

            order.AddDomainEvent(new OrderCreatedEvent(order));

            return order;
        }

        public void Update(OrderName orderName, Address shippingAddress, Address billingAddress, Payment payment)
        {
            ArgumentNullException.ThrowIfNull(orderName);
            ArgumentNullException.ThrowIfNull(shippingAddress);
            ArgumentNullException.ThrowIfNull(billingAddress);
            ArgumentNullException.ThrowIfNull(payment);

            OrderName = orderName;
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            Payment = payment;
        }

        public void Add(Guid productId, int quantity, decimal price)
        {
            var productIdValueObject = ProductId.Of(productId);
            var quantityValueObject = Quantity.Of(quantity);
            var priceValueObject = Price.Of(price);

            var existingItem = _items.FirstOrDefault(x => x.ProductId == productIdValueObject);

            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantityValueObject);
            }
            else
            {
                var orderItem = OrderItem.Create(Id, productIdValueObject, quantityValueObject, priceValueObject);
                _items.Add(orderItem);
            }
        }

        public void Remove(Guid productId)
        {
            var orderItem = _items.FirstOrDefault(x => x.ProductId.Value == productId);
            if (orderItem is not null)
            {
                _items.Remove(orderItem);
            }
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;
            ChangeStatus(OrderStatus.Cancelled);
        }

        public void ChangeStatus(OrderStatus status)
        {
            if (!Enum.IsDefined(status))
            {
                throw new ArgumentOutOfRangeException(nameof(status));
            }

            OrderStatus = status;
        }
    }
}
