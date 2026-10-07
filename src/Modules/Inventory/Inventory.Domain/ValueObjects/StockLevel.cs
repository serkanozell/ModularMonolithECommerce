namespace Inventory.Domain.ValueObjects
{
    public sealed class StockLevel
    {
        public int OnHand { get; }
        public int Reserved { get; }
        public int Available => OnHand - Reserved;

        private StockLevel(int onHand, int reserved)
        {
            if (onHand < 0)
                throw new Exception("On hand stock cannot be negative.");

            if (reserved < 0)
                throw new Exception("Reserved stock cannot be negative.");

            if (reserved > onHand)
                throw new Exception("Reserved stock cannot be greater than on hand stock.");

            OnHand = onHand;
            Reserved = reserved;
        }

        public static StockLevel Create(int onHand) => new(onHand, 0);

        public StockLevel Increase(int quantity)
        {
            EnsurePositive(quantity);

            return new StockLevel(OnHand + quantity, Reserved);
        }

        public StockLevel Decrease(int quantity)
        {
            EnsurePositive(quantity);

            if (quantity > Available)
                throw new Exception($"Cannot decrease stock by {quantity}. Available stock is {Available}.");

            return new StockLevel(OnHand - quantity, Reserved);
        }

        public StockLevel Reserve(int quantity)
        {
            EnsurePositive(quantity);

            if (quantity > Available)
                throw new Exception($"Cannot reserve {quantity} items. Available stock is {Available}.");

            return new StockLevel(OnHand, Reserved + quantity);
        }

        public StockLevel Release(int quantity)
        {
            EnsurePositive(quantity);

            if (quantity > Reserved)
                throw new Exception("Cannot release more stock than reserved.");

            return new StockLevel(OnHand, Reserved - quantity);
        }

        public StockLevel ConfirmReservation(int quantity)
        {
            EnsurePositive(quantity);

            if (quantity > Reserved)
                throw new Exception("Cannot confirm more stock than reserved.");

            return new StockLevel(OnHand - quantity, Reserved - quantity);
        }

        private static void EnsurePositive(int quantity)
        {
            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero.");
        }
    }
}