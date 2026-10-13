namespace Inventory.Application.Features.InventoryItems.ReleaseReserve
{
    public record ReleaseReserveRequest(Guid OrderId);

    public class ReleaseReserveEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/items/stock/release-reservation", async (ReleaseReserveRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new ReleaseReserveCommand(request.OrderId), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("ReleaseReserve")
            .WithTags("Inventory")
            .Produces<ReleaseReserveResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}
