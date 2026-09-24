using Basket.Application.Features.Baskets.Dtos;

namespace Basket.Application.Features.Baskets.CheckoutBasket
{
    public record CheckoutBasketRequest(BasketCheckoutDto BasketCheckout);
    public record CheckoutBasketResponse(bool IsSuccess);

    public class CheckoutBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket/checkout",
                async (CheckoutBasketRequest request, ISender sender) =>
                {
                    var result = await sender.Send(new CheckoutBasketCommand(request.BasketCheckout));

                    return Results.Ok(new CheckoutBasketResponse(result.IsSuccess));
                })
            .WithName("CheckoutBasket")
            .Produces<CheckoutBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Checkout Basket")
            .WithDescription("Checkout Basket");
            //.RequireAuthorization();
        }
    }
}