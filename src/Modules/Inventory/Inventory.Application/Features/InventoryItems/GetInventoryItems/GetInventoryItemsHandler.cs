namespace Inventory.Application.Features.InventoryItems.GetInventoryItems
{
    public record GetInventoryItemsQuery(PaginationRequest PaginationRequest) : IQuery<GetInventoryItemsResult>;

    public record GetInventoryItemsResult(PaginatedResult<InventoryItemDto> Items);

    internal sealed class GetInventoryItemsQueryHandler(IInventoryItemRepository repository) : IQueryHandler<GetInventoryItemsQuery, GetInventoryItemsResult>
    {
        private const int MaxPageSize = 100;

        public async Task<GetInventoryItemsResult> Handle(GetInventoryItemsQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = Math.Max(query.PaginationRequest.PageNumber, 0);
            var pageSize = query.PaginationRequest.PageSize < 1 ? 10 : Math.Min(query.PaginationRequest.PageSize, MaxPageSize);
            var count = await repository.CountAsync(cancellationToken);
            var items = await repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

            return new GetInventoryItemsResult(new PaginatedResult<InventoryItemDto>(pageNumber, pageSize, count, items.ToDtoList()));
        }
    }
}