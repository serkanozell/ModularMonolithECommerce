namespace Inventory.Application.Features.InventoryItems.IncreaseStock
{
    public record IncreaseStockRequest(int Quantity);

    public class IncreaseStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/items/{id:guid}/stock/increase", async (Guid id, IncreaseStockRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new IncreaseStockCommand(id, request.Quantity), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("IncreaseStock")
            .WithTags("Inventory")
            .Produces<IncreaseStockResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}