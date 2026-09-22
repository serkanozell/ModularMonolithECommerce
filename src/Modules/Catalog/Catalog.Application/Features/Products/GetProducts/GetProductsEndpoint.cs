namespace Catalog.Application.Features.Products.GetProducts
{
    public record GetProductsResponse(PaginatedResult<ProductDto> Products);

    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products", async ([AsParameters] PaginationRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetProductsQuery(request), cancellationToken);

                var response = new GetProductsResponse(result.Products);

                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .WithTags("Products")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK);
        }
    }
}