namespace Inventory.Application.Features.InventoryItems
{
    public record InventoryItemDto(Guid Id,
                                   Guid ProductId,
                                   int OnHand,
                                   int Reserved,
                                   int Available,
                                   bool IsInStock,
                                   bool IsActive,
                                   bool IsDeleted,
                                   DateTime? CreatedAt,
                                   DateTime? UpdatedAt);
}