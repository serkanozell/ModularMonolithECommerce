namespace Ordering.Domain.Entities
{
    public class Order : Aggregate<Guid>
    {
        private readonly List<OrderItem> _items = new();
        public IReadOnlyList<OrderItem> Items => _items.AsReadOnly();

        public Guid CustomerId { get; private set; } = default!;
        public string OrderName { get; private set; } = default!;
        public Address ShippingAddress { get; private set; } = default!;
        public Address BillingAddress { get; private set; } = default!;
        public Payment Payment { get; private set; } = default!;
        public decimal TotalPrice => Items.Sum(x => x.Price * x.Quantity);

        private Order() { }

        private Order(Guid customerId, Address shippingAddress, Address billingAddress, Payment payment)
        {
            Id = Guid.NewGuid();
            CustomerId = customerId;
            OrderName = Guid.NewGuid().ToString();
            ShippingAddress = shippingAddress;
            BillingAddress = billingAddress;
            Payment = payment;
            IsActive = true;
            IsDeleted = false;
        }

        public static Order Create(Guid customerId, Address shippingAddress, Address billingAddress, Payment payment)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(customerId, Guid.Empty);
            ArgumentNullException.ThrowIfNull(shippingAddress);
            ArgumentNullException.ThrowIfNull(billingAddress);
            ArgumentNullException.ThrowIfNull(payment);

            var order = new Order(customerId, shippingAddress, billingAddress, payment);

            order.AddDomainEvent(new OrderCreatedEvent(order));

            return order;
        }

        public void Update(string orderName, Address shippingAddress, Address billingAddress, Payment payment)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(orderName);
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
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(price);

            var existingItem = _items.FirstOrDefault(x => x.ProductId == productId);

            if (existingItem != null)
            {
                existingItem.IncreaseQuantity(quantity);
            }
            else
            {
                var orderItem = OrderItem.Create(Id, productId, quantity, price);
                _items.Add(orderItem);
            }
        }

        public void Remove(Guid productId)
        {
            var orderItem = _items.FirstOrDefault(x => x.ProductId == productId);
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
        }
    }
}
