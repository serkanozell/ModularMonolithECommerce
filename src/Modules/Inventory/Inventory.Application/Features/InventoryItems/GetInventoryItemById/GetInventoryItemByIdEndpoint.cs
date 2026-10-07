namespace Inventory.Application.Features.InventoryItems.GetInventoryItemById
{
    public record GetInventoryItemByIdResponse(InventoryItemDto Item);

    public class GetInventoryItemByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/inventory/items/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetInventoryItemByIdQuery(id), cancellationToken);
                return Results.Ok(new GetInventoryItemByIdResponse(result.Item));
            })
            .WithName("GetInventoryItemById")
            .WithTags("Inventory")
            .Produces<GetInventoryItemByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}