namespace Ordering.Application.Features.Orders.GetOrders
{
    public record GetOrdersResponse(PaginatedResult<OrderDto> Orders);

    public class GetOrdersEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/orders", async ([AsParameters] PaginationRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetOrdersQuery(request), cancellationToken);

                return Results.Ok(new GetOrdersResponse(result.Orders));
            })
            .WithName("GetOrders")
            .WithTags("Orders")
            .Produces<GetOrdersResponse>(StatusCodes.Status200OK);
        }
    }
}
