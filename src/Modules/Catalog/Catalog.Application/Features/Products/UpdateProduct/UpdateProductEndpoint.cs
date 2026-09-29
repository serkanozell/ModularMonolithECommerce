namespace Catalog.Application.Features.Products.UpdateProduct
{
    public record UpdateProductRequest(string Name,
                                       List<string> Category,
                                       string? Description,
                                       decimal Price);

    public record UpdateProductResponse(bool IsSuccess);

    public class UpdateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/products/{id:guid}", async (Guid id, UpdateProductRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new UpdateProductCommand(id,
                                                       request.Name,
                                                       request.Category,
                                                       request.Description,
                                                       request.Price);

                var result = await sender.Send(command, cancellationToken);

                return Results.Ok(new UpdateProductResponse(result.IsSuccess));
            })
            .WithName("UpdateProduct")
            .WithTags("Products")
            .Produces<UpdateProductResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound)
            .RequireAuthorization();
        }
    }
}