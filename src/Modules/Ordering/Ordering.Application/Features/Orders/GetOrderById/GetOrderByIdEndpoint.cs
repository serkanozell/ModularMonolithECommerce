namespace Ordering.Application.Features.Orders.GetOrderById
{
    public record GetOrderByIdResponse(OrderDto Order);

    public class GetOrderByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/orders/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetOrderByIdQuery(id), cancellationToken);

                return Results.Ok(new GetOrderByIdResponse(result.Order));
            })
            .WithName("GetOrderById")
            .WithTags("Orders")
            .Produces<GetOrderByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
