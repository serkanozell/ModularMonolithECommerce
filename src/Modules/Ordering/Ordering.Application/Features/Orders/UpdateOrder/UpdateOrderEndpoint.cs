namespace Ordering.Application.Features.Orders.UpdateOrder
{
    public record UpdateOrderRequest(string OrderName,
                                     AddressDto ShippingAddress,
                                     AddressDto BillingAddress,
                                     PaymentDto Payment);

    public record UpdateOrderResponse(bool IsSuccess);

    public class UpdateOrderEndpoint : ICarterModule
    {
        public void AddRoutes(IEndpointRouteBuilder app)
        {
            app.MapPut("/api/orders/{id:guid}", async (Guid id, UpdateOrderRequest request, ISender sender, CancellationToken cancellationToken) =>
            {
                var command = new UpdateOrderCommand(id,
                                                     request.OrderName,
                                                     request.ShippingAddress,
                                                     request.BillingAddress,
                                                     request.Payment);

                var result = await sender.Send(command, cancellationToken);

                return Results.Ok(new UpdateOrderResponse(result.IsSuccess));
            })
            .WithName("UpdateOrder")
            .WithTags("Orders")
            .Produces<UpdateOrderResponse>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status400BadRequest)
            .ProducesProblem(StatusCodes.Status404NotFound);
        }
    }
}
