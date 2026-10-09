using BuildingBlocks.Shared.Dtos;

namespace Ordering.Application.Features.Orders.CreateOrder
{
    public record CreateOrderCommand(Guid CustomerId,
                                     AddressDto ShippingAddress,
                                     AddressDto BillingAddress,
                                     PaymentDto Payment,
                                     List<OrderItemDto> Items) : ICommand<CreateOrderResult>;

    public record CreateOrderResult(Guid Id);

    public class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
    {
        public CreateOrderCommandValidator()
        {
            RuleFor(x => x.CustomerId)
                .NotEmpty().WithMessage("CustomerId is required.");

            RuleFor(x => x.ShippingAddress)
                .NotNull().WithMessage("ShippingAddress is required.");

            RuleFor(x => x.BillingAddress)
                .NotNull().WithMessage("BillingAddress is required.");

            RuleFor(x => x.Payment)
                .NotNull().WithMessage("Payment is required.");

            RuleFor(x => x.Items)
                .NotEmpty().WithMessage("Order must contain at least one item.");

            RuleForEach(x => x.Items).ChildRules(item =>
            {
                item.RuleFor(i => i.ProductId).NotEmpty().WithMessage("ProductId is required.");
                item.RuleFor(i => i.Quantity).GreaterThan(0).WithMessage("Quantity must be greater than zero.");
                item.RuleFor(i => i.Price).GreaterThan(0).WithMessage("Price must be greater than zero.");
            });
        }
    }

    internal sealed class CreateOrderCommandHandler(IOrderRepository repository) : ICommandHandler<CreateOrderCommand, CreateOrderResult>
    {
        public async Task<CreateOrderResult> Handle(CreateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = Order.Create(CustomerId.Of(command.CustomerId),
                                     command.ShippingAddress.ToValueObject(),
                                     command.BillingAddress.ToValueObject(),
                                     command.Payment.ToValueObject());



            foreach (var item in command.Items)
            {
                order.Add(item.ProductId, item.Quantity, item.Price);
            }

            repository.Add(order);

            await repository.SaveChangesAsync(cancellationToken);

            return new CreateOrderResult(order.Id.Value);
        }
    }
}
