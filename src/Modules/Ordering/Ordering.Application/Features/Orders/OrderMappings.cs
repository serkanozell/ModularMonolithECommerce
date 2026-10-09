using BuildingBlocks.Shared.Dtos;

namespace Ordering.Application.Features.Orders
{
    public static class OrderMappings
    {
        public static OrderDto ToDto(this Order order) =>
            new(order.Id.Value,
                order.CustomerId.Value,
                order.OrderName.Value,
                order.ShippingAddress.ToDto(),
                order.BillingAddress.ToDto(),
                order.Payment.ToDto(),
                order.TotalPrice,
                order.Items.Select(i => i.ToDto()).ToList(),
                order.OrderStatus);

        public static List<OrderDto> ToDtoList(this IEnumerable<Order> orders) =>
            orders.Select(o => o.ToDto()).ToList();

        public static OrderItemDto ToDto(this OrderItem item) =>
            new(item.ProductId.Value, item.Quantity.Value, item.Price.Value);

        public static AddressDto ToDto(this Address address) =>
            new(address.FirstName,
                address.LastName,
                address.EmailAddress ?? string.Empty,
                address.AddressLine,
                address.Country,
                address.State,
                address.ZipCode);

        public static PaymentDto ToDto(this Payment payment) =>
            new(payment.CardName ?? string.Empty,
                payment.CardNumber,
                payment.Expiration,
                payment.CVV,
                payment.PaymentMethod);

        public static Address ToValueObject(this AddressDto dto) =>
            Address.Of(dto.FirstName,
                       dto.LastName,
                       dto.EmailAddress,
                       dto.AddressLine,
                       dto.Country,
                       dto.State,
                       dto.ZipCode);

        public static Payment ToValueObject(this PaymentDto dto) =>
            Payment.Of(dto.CardName,
                       dto.CardNumber,
                       dto.Expiration,
                       dto.Cvv,
                       dto.PaymentMethod);
    }
}
