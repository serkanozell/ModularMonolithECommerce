using Inventory.Domain.Enums;
using Inventory.Domain.ValueObjects;

namespace Inventory.Domain.Entities
{
    public sealed class StockReservation : Entity<Guid>
    {
        public InventoryItemId InventoryItemId { get; private set; }
        public OrderId OrderId { get; private set; }
        public Quantity Quantity { get; private set; }
        public StockReservationStatus Status { get; private set; }

        private StockReservation() { }

        private StockReservation(InventoryItemId inventoryItemId, OrderId orderId, Quantity quantity)
        {
            InventoryItemId = inventoryItemId;
            OrderId = orderId;
            Quantity = quantity;
            Status = StockReservationStatus.Reserved;
            IsActive = true;
            IsDeleted = false;
        }

        internal static StockReservation Create(InventoryItemId inventoryItemId, OrderId orderId, Quantity quantity)
        {
            ArgumentNullException.ThrowIfNull(inventoryItemId);
            ArgumentNullException.ThrowIfNull(orderId);
            ArgumentNullException.ThrowIfNull(quantity);

            return new StockReservation(inventoryItemId, orderId, quantity);
        }

        internal void Release()
        {
            Status = StockReservationStatus.Released;
            IsActive = false;
            IsDeleted = true;
        }

        internal void Confirm()
        {
            Status = StockReservationStatus.Confirmed;
        }
    }
}