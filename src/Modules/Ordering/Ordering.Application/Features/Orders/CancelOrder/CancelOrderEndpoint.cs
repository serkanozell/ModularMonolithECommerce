namespace Ordering.Application.Features.Orders.CancelOrder
{
    public record CancelOrderRequest(Guid OrderId, string Reason);

    public record CancelOrderResponse(bool IsSuccess);

    public class CancelOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/orders/cancel", async (CancelOrderRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new CancelOrderCommand(request.OrderId, request.Reason);
                
                var result = await sender.Send(command, cancellationToken);
                
                var response = new CancelOrderResponse(result.IsSuccess);
                
                return Results.Ok(response);
            })
            .WithName("CancelOrder")
            .WithTags("Orders")
            .Produces<CancelOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}