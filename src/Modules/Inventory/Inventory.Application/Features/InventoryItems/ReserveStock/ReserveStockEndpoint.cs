using BuildingBlocks.Shared.Dtos;

namespace Inventory.Application.Features.InventoryItems.ReserveStock
{
    public record ReserveStockRequest(List<OrderItemDto> OrderItems);

    public class ReserveStockEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/orders/{orderId:guid}/stock/reserve", async (Guid orderId, ReserveStockRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new ReserveStockCommand(request.OrderItems, orderId), cancellationToken);
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
