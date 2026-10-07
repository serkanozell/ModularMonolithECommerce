namespace Inventory.Application.Features.InventoryItems
{
    public static class InventoryItemMappings
    {
        public static InventoryItemDto ToDto(this InventoryItem item) =>
            new(item.Id,
                item.ProductId,
                item.StockLevel.OnHand,
                item.StockLevel.Reserved,
                item.StockLevel.Available,
                item.IsInStock,
                item.IsActive,
                item.IsDeleted,
                item.CreatedAt,
                item.UpdatedAt);

        public static List<InventoryItemDto> ToDtoList(this IEnumerable<InventoryItem> items) =>
            items.Select(item => item.ToDto()).ToList();
    }
}