namespace Catalog.Application.Features.Products.CreateProduct
{
    public record CreateProductRequest(string Name,
                                       List<string> Category,
                                       string? Description,
                                       decimal Price,
                                       int StockQuantity);

    public record CreateProductResponse(Guid Id);

    public class CreateProductEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/products", async (CreateProductRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new CreateProductCommand(request.Name,
                                                       request.Category,
                                                       request.Description,
                                                       request.Price,
                                                       request.StockQuantity);

                var result = await sender.Send(command, cancellationToken);

                var response = new CreateProductResponse(result.Id);

                return Results.Created($"/api/products/{response.Id}", response);
            })
            .WithName("CreateProduct")
            .WithTags("Products")
            .Produces<CreateProductResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .RequireAuthorization();
        }
    }
}
