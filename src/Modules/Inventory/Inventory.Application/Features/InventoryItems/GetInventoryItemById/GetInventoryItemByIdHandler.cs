namespace Inventory.Application.Features.InventoryItems.GetInventoryItemById
{
    public record GetInventoryItemByIdQuery(Guid Id) : IQuery<GetInventoryItemByIdResult>;

    public record GetInventoryItemByIdResult(InventoryItemDto Item);

    internal sealed class GetInventoryItemByIdQueryHandler(IInventoryItemRepository repository) : IQueryHandler<GetInventoryItemByIdQuery, GetInventoryItemByIdResult>
    {
        public async Task<GetInventoryItemByIdResult> Handle(GetInventoryItemByIdQuery query, CancellationToken cancellationToken)
        {
            if (query.Id == Guid.Empty)
                throw new ArgumentException("Id is required.", nameof(query));

            var item = await repository.GetByIdAsync(query.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Inventory item with id '{query.Id}' was not found.");

            return new GetInventoryItemByIdResult(item.ToDto());
        }
    }
}