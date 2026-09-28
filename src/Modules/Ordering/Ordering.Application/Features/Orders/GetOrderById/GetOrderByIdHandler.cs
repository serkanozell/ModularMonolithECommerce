namespace Ordering.Application.Features.Orders.GetOrderById
{
    public record GetOrderByIdQuery(Guid Id) : IQuery<GetOrderByIdResult>;

    public record GetOrderByIdResult(OrderDto Order);

    public class GetOrderByIdQueryHandler(IOrderRepository repository) : IQueryHandler<GetOrderByIdQuery, GetOrderByIdResult>
    {
        public async Task<GetOrderByIdResult> Handle(GetOrderByIdQuery query, CancellationToken cancellationToken)
        {
            if (query.Id == Guid.Empty)
                throw new ArgumentException("Id is required.", nameof(query.Id));

            var order = await repository.GetByIdAsync(query.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with id '{query.Id}' was not found.");

            return new GetOrderByIdResult(order.ToDto());
        }
    }
}
