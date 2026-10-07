namespace Inventory.Application.Features.InventoryItems.ReleaseReserve
{
    public record ReleaseReserveRequest(Guid OrderId);

    public class ReleaseReserveEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/items/{id:guid}/stock/release-reservation", async (Guid id, ReleaseReserveRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new ReleaseReserveCommand(id, request.OrderId), cancellationToken);
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
