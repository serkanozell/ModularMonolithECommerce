namespace Basket.Application.Features.Baskets.GetBasketByUserName
{
    public record GetBasketByUserNameResponse(BasketDto Basket);

    public class GetBasketByUserNameEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapGet("/api/baskets/{userName}", async (string userName, ISender sender, CancellationToken cancellationToken) =>
            {
                var result = await sender.Send(new GetBasketByUserNameQuery(userName), cancellationToken);

                var response = new GetBasketByUserNameResponse(result.Basket);

                return Results.Ok(response);
            })
            .WithName("GetBasketByUserName")
            .WithTags("Baskets")
            .WithSummary("Get Basket")
            .WithDescription("Get Basket")
            .Produces<GetBasketByUserNameResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
