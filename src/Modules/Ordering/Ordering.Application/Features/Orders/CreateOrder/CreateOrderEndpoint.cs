using BuildingBlocks.Shared.Dtos;

namespace Ordering.Application.Features.Orders.CreateOrder
{
    public record CreateOrderRequest(Guid CustomerId,
                                     AddressDto ShippingAddress,
                                     AddressDto BillingAddress,
                                     PaymentDto Payment,
                                     List<OrderItemDto> Items);

    public record CreateOrderResponse(Guid Id);

    public class CreateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPost("/api/orders", async (CreateOrderRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new CreateOrderCommand(request.CustomerId,
                                                     request.ShippingAddress,
                                                     request.BillingAddress,
                                                     request.Payment,
                                                     request.Items);

                var result = await sender.Send(command, cancellationToken);

                var response = new CreateOrderResponse(result.Id);

                return Results.Created($"/api/orders/{response.Id}", response);
            })
            .WithName("CreateOrder")
            .WithTags("Orders")
            .Produces<CreateOrderResponse>(StatusCodes.Status201Created)
            .ProducesProblem(StatusCodes.Status400BadRequest);
        }
    }
}
