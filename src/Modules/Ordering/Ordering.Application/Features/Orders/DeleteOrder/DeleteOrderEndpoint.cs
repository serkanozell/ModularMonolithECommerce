namespace Ordering.Application.Features.Orders.DeleteOrder
{
    public record DeleteOrderResponse(bool IsSuccess);

    public class DeleteOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/orders/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteOrderCommand(id), cancellationToken);

                return Results.Ok(new DeleteOrderResponse(result.IsSuccess));
            })
            .WithName("DeleteOrder")
            .WithTags("Orders")
            .Produces<DeleteOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
