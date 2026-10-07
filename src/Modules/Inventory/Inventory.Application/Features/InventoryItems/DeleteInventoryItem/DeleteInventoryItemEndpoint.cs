namespace Inventory.Application.Features.InventoryItems.DeleteInventoryItem
{
    public class DeleteInventoryItemEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/inventory/items/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteInventoryItemCommand(id), cancellationToken);
                return Results.Ok(result);
            })
            .WithName("DeleteInventoryItem")
            .WithTags("Inventory")
            .Produces<DeleteInventoryItemResult>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}