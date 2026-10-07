namespace Inventory.Application.Features.InventoryItems.CreateInventoryItem
{
    public record CreateInventoryItemRequest(Guid ProductId, int InitialQuantity = 0);

    public record CreateInventoryItemResponse(Guid Id);

    public class CreateInventoryItemEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/inventory/items", async (CreateInventoryItemRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new CreateInventoryItemCommand(request.ProductId, request.InitialQuantity), cancellationToken);
                var response = new CreateInventoryItemResponse(result.Id);
                return Results.Created($"/api/inventory/items/{response.Id}", response);
            })
            .WithName("CreateInventoryItem")
            .WithTags("Inventory")
            .Produces<CreateInventoryItemResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
        }
    }
}