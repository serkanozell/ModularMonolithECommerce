namespace Inventory.Application.Features.InventoryItems.ConfirmReservation
{
    public record ConfirmReservationRequest(Guid OrderId);

    public class ConfirmReservationEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/items/{id:guid}/stock/confirm-reservation", async (Guid id, ConfirmReservationRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new ConfirmReservationCommand(id, request.OrderId), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("ConfirmReservation")
            .WithTags("Inventory")
            .Produces<ConfirmReservationResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}
