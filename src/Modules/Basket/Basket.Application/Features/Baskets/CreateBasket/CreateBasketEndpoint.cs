namespace Basket.Application.Features.Baskets.CreateBasket
{
    public record CreateBasketRequest(BasketDto BasketDto);

    public record CreateBasketResponse(Guid Id);

    public class CreateBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/baskets", async (CreateBasketRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new CreateBasketCommand(request.BasketDto);

                var result = await sender.Send(command, cancellationToken);

                var response = new CreateBasketResponse(result.Id);

                return Results.Created($"/api/baskets/{response.Id}", response);
            })
            .WithName("Create Basket")
            .WithTags("Baskets")
            .WithSummary("Create Basket")
            .WithDescription("Create Basket")
            .Produces<CreateBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
