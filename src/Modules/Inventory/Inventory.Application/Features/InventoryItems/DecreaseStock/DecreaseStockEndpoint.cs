namespace Inventory.Application.Features.InventoryItems.DecreaseStock
{
    public record DecreaseStockRequest(int Quantity);

    public class DecreaseStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/items/{id:guid}/stock/decrease", async (Guid id, DecreaseStockRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DecreaseStockCommand(id, request.Quantity), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("DecreaseStock")
            .WithTags("Inventory")
            .Produces<DecreaseStockResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}