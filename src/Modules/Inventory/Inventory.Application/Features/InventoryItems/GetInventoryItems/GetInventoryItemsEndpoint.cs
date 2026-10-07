namespace Inventory.Application.Features.InventoryItems.GetInventoryItems
{
    public record GetInventoryItemsResponse(PaginatedResult<InventoryItemDto> Items);

    public class GetInventoryItemsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/inventory/items", async ([AsParameters] PaginationRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetInventoryItemsQuery(request), cancellationToken);
                return Results.Ok(new GetInventoryItemsResponse(result.Items));
            })
            .WithName("GetInventoryItems")
            .WithTags("Inventory")
            .Produces<GetInventoryItemsResponse>(StatusCodes.Status200OK);
        }
    }
}