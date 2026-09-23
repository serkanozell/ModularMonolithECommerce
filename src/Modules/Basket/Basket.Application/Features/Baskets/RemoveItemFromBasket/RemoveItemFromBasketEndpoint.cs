using Microsoft.AspNetCore.Mvc;

namespace Basket.Application.Features.Baskets.RemoveItemFromBasket
{
    public record RemoveItemFromBasketResponse(Guid Id);

    public class RemoveItemFromBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapDelete("/basket/{userName}/items/{productId}",
                async ([FromRoute] string userName,
                       [FromRoute] Guid productId,
                       ISender sender) =>
                {
                    var command = new RemoveItemFromBasketCommand(userName, productId);

                    var result = await sender.Send(command);

                    return Results.Ok(new RemoveItemFromBasketResponse(result.Id));
                })
            .Produces<RemoveItemFromBasketResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Remove Item From Basket")
            .WithDescription("Remove Item From Basket")
            .RequireAuthorization();
        }
    }
}