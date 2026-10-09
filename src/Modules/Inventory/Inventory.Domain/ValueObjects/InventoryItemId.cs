namespace Inventory.Domain.ValueObjects
{
    public record InventoryItemId
    {
        public Guid Value { get; }

        private InventoryItemId(Guid value) => Value = value;

        public static InventoryItemId Of(Guid value)
        {
            ArgumentOutOfRangeException.ThrowIfEqual(value, Guid.Empty);

            return new InventoryItemId(value);
        }
    }
}