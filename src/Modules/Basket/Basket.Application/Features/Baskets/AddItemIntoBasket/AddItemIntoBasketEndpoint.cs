using Basket.Application.Features.Baskets.Dtos;
using Microsoft.AspNetCore.Mvc;

namespace Basket.Application.Features.Baskets.AddItemIntoBasket
{
    public record AddItemIntoBasketRequest(string UserName, BasketItemDto BasketItemDto);
    public record AddItemIntoBasketResponse(Guid Id);

    public class AddItemIntoBasketEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/basket/{userName}/items",
                async ([FromRoute] string userName,
                       [FromBody] AddItemIntoBasketRequest request,
                       ISender sender) =>
                {
                    var command = new AddItemIntoBasketCommand(userName, request.BasketItemDto);

                    var result = await sender.Send(command);

                    var response = new AddItemIntoBasketResponse(result.Id);

                    return Results.Created($"/basket/{response.Id}", response);
                })
            .Produces<AddItemIntoBasketResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .WithSummary("Add Item Into Basket")
            .WithDescription("Add Item Into Basket")
            .RequireAuthorization();
        }
    }
}