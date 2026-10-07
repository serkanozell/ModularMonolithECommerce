namespace Inventory.Application.Features.InventoryItems.ReserveStock
{
    public record ReserveStockRequest(Guid OrderId, int Quantity);

    public class ReserveStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/items/{id:guid}/stock/reserve", async (Guid id, ReserveStockRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new ReserveStockCommand(id, request.OrderId, request.Quantity), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("ReserveStock")
            .WithTags("Inventory")
            .Produces<ReserveStockResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}
