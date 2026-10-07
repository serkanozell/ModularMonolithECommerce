using Inventory.Domain.Enums;

namespace Inventory.Domain.Entities
{
    public sealed class StockReservation : Entity<Guid>
    {
        public Guid InventoryItemId { get; private set; }
        public Guid OrderId { get; private set; }
        public int Quantity { get; private set; }
        public StockReservationStatus Status { get; private set; }

        private StockReservation() { }

        private StockReservation(Guid inventoryItemId, Guid orderId, int quantity)
        {
            InventoryItemId = inventoryItemId;
            OrderId = orderId;
            Quantity = quantity;
            Status = StockReservationStatus.Reserved;
            IsActive = true;
            IsDeleted = false;
        }

        internal static StockReservation Create(Guid inventoryItemId, Guid orderId, int quantity)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(inventoryItemId, Guid.Empty);
            ArgumentOutOfRangeException.ThrowIfEqual(orderId, Guid.Empty);
            ArgumentOutOfRangeException.ThrowIfNegativeOrZero(quantity);

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