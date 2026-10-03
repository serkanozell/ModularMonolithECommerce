namespace Ordering.Application.Features.Orders.GetOrders
{
    public record GetOrdersQuery(PaginationRequest PaginationRequest) : IQuery<GetOrdersResult>;

    public record GetOrdersResult(PaginatedResult<OrderDto> Orders);

    internal sealed class GetOrdersQueryHandler(IOrderRepository repository) : IQueryHandler<GetOrdersQuery, GetOrdersResult>
    {
        private const int MaxPageSize = 100;

        public async Task<GetOrdersResult> Handle(GetOrdersQuery query, CancellationToken cancellationToken)
        {
            var pageNumber = query.PaginationRequest.PageNumber < 0 ? 0 : query.PaginationRequest.PageNumber;
            var pageSize = query.PaginationRequest.PageSize < 1 ? 10 : Math.Min(query.PaginationRequest.PageSize, MaxPageSize);

            var totalCount = await repository.CountAsync(cancellationToken);
            var orders = await repository.GetPagedAsync(pageNumber, pageSize, cancellationToken);

            return new GetOrdersResult(new PaginatedResult<OrderDto>(pageNumber, pageSize, totalCount, orders.ToDtoList()));
        }
    }
}
