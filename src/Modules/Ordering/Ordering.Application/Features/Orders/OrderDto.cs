using BuildingBlocks.Shared.Dtos;

namespace Ordering.Application.Features.Orders
{
    public record AddressDto(string FirstName,
                             string LastName,
                             string EmailAddress,
                             string AddressLine,
                             string Country,
                             string State,
                             string ZipCode);

    public record PaymentDto(string CardName,
                             string CardNumber,
                             string Expiration,
                             string Cvv,
                             int PaymentMethod);

    public record OrderDto(Guid Id,
                           Guid CustomerId,
                           string OrderName,
                           AddressDto ShippingAddress,
                           AddressDto BillingAddress,
                           PaymentDto Payment,
                           decimal TotalPrice,
                           List<OrderItemDto> Items);
}
