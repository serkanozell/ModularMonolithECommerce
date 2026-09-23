using Basket.Domain.Events;

namespace Basket.Domain.Entities
{
    public class ShoppingCart : Aggregate<Guid>
    {
        public string UserName { get; private set; } = default!;

        private readonly List<ShoppingCartItem> _items = new();
        public IReadOnlyList<ShoppingCartItem> Items => _items.AsReadOnly();

        public decimal TotalPrice => _items.Sum(i => i.Price * i.Quantity);

        private ShoppingCart(string userName)
        {
            Id = Guid.NewGuid();
            UserName = userName;
            IsActive = true;
            IsDeleted = false;
        }

        public static ShoppingCart Create(string userName)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(userName);

            var cart = new ShoppingCart(userName);

            cart.AddDomainEvent(new BasketCreatedEvent(cart.Id, cart.UserName));

            return cart;
        }

        public void AddItem(Guid productId, int quantity, string color, decimal price, string productName)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);
            ArgumentOutOfRangeException.ThrowIfNegative(price);
            ArgumentException.ThrowIfNullOrWhiteSpace(color);
            ArgumentException.ThrowIfNullOrWhiteSpace(productName);

            var existingItem = _items.FirstOrDefault(i => i.ProductId == productId && i.Color == color);

            if (existingItem is not null)
                existingItem.Quantity += quantity;
            else
                _items.Add(new ShoppingCartItem(Id, productId, quantity, color, price, productName));

            AddDomainEvent(new BasketItemAddedEvent(Id, productId, quantity, price, productName));
        }

        public void RemoveItem(Guid productId)
        {
            var item = _items.FirstOrDefault(i => i.ProductId == productId);

            if (item is null)
                throw new InvalidOperationException($"Product '{productId}' is not in the cart.");

            _items.Remove(item);

            AddDomainEvent(new BasketItemRemovedEvent(Id, productId));
        }

        public void UpdateItemQuantity(Guid productId, int quantity)
        {
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

            var item = _items.FirstOrDefault(i => i.ProductId == productId);

            if (item is null)
                throw new InvalidOperationException($"Product '{productId}' is not in the cart.");

            if (item.Quantity == quantity)
                return;

            item.Quantity = quantity;
        }

        public void ClearItems() => _items.Clear();

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