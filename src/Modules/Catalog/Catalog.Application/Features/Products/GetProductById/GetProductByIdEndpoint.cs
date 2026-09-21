namespace Catalog.Application.Features.Products.GetProductById
{
    public record GetProductByIdResponse(ProductDto Product);

    public class GetProductByIdEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products/{id:guid}", async (Guid id, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetProductByIdQuery(id), cancellationToken);

                return Results.Ok(new GetProductByIdResponse(result.Product));
            })
            .WithName("GetProductById")
            .WithTags("Products")
            .Produces<GetProductByIdResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}