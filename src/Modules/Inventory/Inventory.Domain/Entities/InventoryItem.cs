using Inventory.Domain.Enums;
using Inventory.Domain.Events;
using Inventory.Domain.ValueObjects;

namespace Inventory.Domain.Entities
{
    public sealed class InventoryItem : Aggregate<InventoryItemId>
    {
        private readonly List<StockReservation> _reservations = new();

        public ProductId ProductId { get; private set; }
        public StockLevel StockLevel { get; private set; }
        public IReadOnlyCollection<StockReservation> Reservations => _reservations.AsReadOnly();
        public bool IsInStock => StockLevel.Available > 0;

        private InventoryItem() { }

        private InventoryItem(ProductId productId, StockLevel stockLevel)
        {
            Id = InventoryItemId.Of(Guid.NewGuid());
            ProductId = productId;
            StockLevel = stockLevel;
            IsActive = true;
            IsDeleted = false;
        }

        public static InventoryItem Create(ProductId productId, int initialQuantity = 0)
        {
            ArgumentNullException.ThrowIfNull(productId);
            ArgumentOutOfRangeException.ThrowIfNegative(initialQuantity);

            var item = new InventoryItem(productId, StockLevel.Create(initialQuantity));
            item.AddDomainEvent(new InventoryItemCreatedEvent(item.Id, productId, initialQuantity));
            item.AddDomainEvent(new StockAvailabilityChangedEvent(item.Id, productId, item.IsInStock, item.StockLevel.Available));

            return item;
        }

        public void IncreaseStock(int quantity)
        {
            UpdateStockLevel(StockLevel.Increase(quantity));
        }

        public void DecreaseStock(int quantity)
        {
            UpdateStockLevel(StockLevel.Decrease(quantity));
        }

        public bool Reserve(Guid orderId, int quantity)
        {
            var orderIdValueObject = OrderId.Of(orderId);
            var quantityValueObject = Quantity.Of(quantity);

            var existingReservation = _reservations.FirstOrDefault(reservation => reservation.OrderId == orderIdValueObject && reservation.IsActive && !reservation.IsDeleted);
            if (existingReservation is not null)
            {
                if (existingReservation.Status == StockReservationStatus.Confirmed)
                    return false;

                if (existingReservation.Status == StockReservationStatus.Released)
                    return false;

                return existingReservation.Quantity == quantityValueObject;
            }

            UpdateStockLevel(StockLevel.Reserve(quantityValueObject.Value));
            _reservations.Add(StockReservation.Create(Id, orderIdValueObject, quantityValueObject));
            return true;
        }

        public bool ReleaseReservation(Guid orderId)
        {
            var orderIdValueObject = OrderId.Of(orderId);
            var reservation = _reservations.FirstOrDefault(reservation => reservation.OrderId == orderIdValueObject && reservation.IsActive && !reservation.IsDeleted);
            if (reservation is null || reservation.Status != StockReservationStatus.Reserved)
                return false;

            UpdateStockLevel(StockLevel.Release(reservation.Quantity.Value));
            reservation.Release();
            return true;
        }

        public bool ConfirmReservation(Guid orderId)
        {
            var orderIdValueObject = OrderId.Of(orderId);
            var reservation = _reservations.FirstOrDefault(reservation => reservation.OrderId == orderIdValueObject && reservation.IsActive && !reservation.IsDeleted);
            if (reservation is null)
                return false;

            if (reservation.Status == StockReservationStatus.Confirmed)
                return true;

            if (reservation.Status == StockReservationStatus.Released)
                return false;

            UpdateStockLevel(StockLevel.ConfirmReservation(reservation.Quantity.Value));
            reservation.Confirm();
            return true;
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

        private void UpdateStockLevel(StockLevel newStockLevel)
        {
            var previousAvailableQuantity = StockLevel.Available;
            StockLevel = newStockLevel;

            if (previousAvailableQuantity != StockLevel.Available)
            {
                AddDomainEvent(new StockAvailabilityChangedEvent(
                    Id, ProductId, IsInStock, StockLevel.Available));
            }
        }
    }
}