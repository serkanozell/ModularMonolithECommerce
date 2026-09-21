namespace Catalog.Application.Features.Products.GetProducts
{
    public record GetProductsRequest(int PageNumber = 1, int PageSize = 10);

    public record GetProductsResponse(int PageNumber,
                                      int PageSize,
                                      int TotalCount,
                                      List<ProductDto> Products);

    public class GetProductsEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/products", async ([AsParameters] GetProductsRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetProductsQuery(request.PageNumber, request.PageSize), cancellationToken);

                var response = new GetProductsResponse(result.PageNumber,
                                                       result.PageSize,
                                                       result.TotalCount,
                                                       result.Products);

                return Results.Ok(response);
            })
            .WithName("GetProducts")
            .WithTags("Products")
            .Produces<GetProductsResponse>(StatusCodes.Status200OK);
        }
    }
}