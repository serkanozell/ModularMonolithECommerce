using Inventory.Domain.Enums;
using Inventory.Domain.Events;
using Inventory.Domain.ValueObjects;

namespace Inventory.Domain.Entities
{
    public sealed class InventoryItem : Aggregate<Guid>
    {
        private readonly List<StockReservation> _reservations = new();

        public Guid ProductId { get; private set; }
        public StockLevel StockLevel { get; private set; }
        public IReadOnlyCollection<StockReservation> Reservations => _reservations.AsReadOnly();
        public bool IsInStock => StockLevel.Available > 0;

        private InventoryItem() { }

        private InventoryItem(Guid productId, StockLevel stockLevel)
        {
            Id = Guid.NewGuid();
            ProductId = productId;
            StockLevel = stockLevel;
            IsActive = true;
            IsDeleted = false;
        }

        public static InventoryItem Create(Guid productId, int initialQuantity = 0)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(productId, Guid.Empty);
            ArgumentOutOfRangeException.ThrowIfNegative(initialQuantity);

            var item = new InventoryItem(productId, StockLevel.Create(initialQuantity));
            item.AddDomainEvent(new InventoryItemCreatedEvent(item.Id, productId, initialQuantity));

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
            ArgumentOutOfRangeException.ThrowIfEqual(orderId, Guid.Empty);

            var existingReservation = _reservations.FirstOrDefault(reservation => reservation.OrderId == orderId && reservation.IsActive && !reservation.IsDeleted);
            if (existingReservation is not null)
            {
                if (existingReservation.Status == StockReservationStatus.Confirmed)
                    return false;

                if (existingReservation.Status == StockReservationStatus.Released)
                    return false;

                return existingReservation.Quantity == quantity;
            }

            UpdateStockLevel(StockLevel.Reserve(quantity));
            _reservations.Add(StockReservation.Create(Id, orderId, quantity));
            return true;
        }

        public bool ReleaseReservation(Guid orderId)
        {
            var reservation = _reservations.FirstOrDefault(reservation => reservation.OrderId == orderId && reservation.IsActive && !reservation.IsDeleted);
            if (reservation is null || reservation.Status != StockReservationStatus.Reserved)
                return false;

            UpdateStockLevel(StockLevel.Release(reservation.Quantity));
            reservation.Release();
            return true;
        }

        public bool ConfirmReservation(Guid orderId)
        {
            var reservation = _reservations.FirstOrDefault(reservation => reservation.OrderId == orderId && reservation.IsActive && !reservation.IsDeleted);
            if (reservation is null)
                return false;

            if (reservation.Status == StockReservationStatus.Confirmed)
                return true;

            if (reservation.Status == StockReservationStatus.Released)
                return false;

            UpdateStockLevel(StockLevel.ConfirmReservation(reservation.Quantity));
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
            var wasInStock = IsInStock;
            StockLevel = newStockLevel;

            if (wasInStock != IsInStock)
            {
                AddDomainEvent(new StockAvailabilityChangedEvent(
                    Id, ProductId, IsInStock, StockLevel.Available));
            }
        }
    }
}