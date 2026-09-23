namespace Basket.Application.Features.Baskets.DeleteBasket
{
    public record DeleteBasketResponse(bool IsSuccess);

    public class DeleteBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/api/baskets/{userName}", async (string userName, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new DeleteBasketCommand(userName), cancellationToken);

                var response = new DeleteBasketResponse(result.IsSuccess);

                return Results.Ok(response);
            })
            .WithName("DeleteBasket")
            .WithTags("Baskets")
            .WithSummary("Delete Basket")
            .WithDescription("Delete Basket")
            .Produces<DeleteBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
