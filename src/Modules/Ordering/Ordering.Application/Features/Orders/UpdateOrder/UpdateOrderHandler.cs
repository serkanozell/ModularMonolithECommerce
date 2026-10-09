namespace Ordering.Application.Features.Orders.UpdateOrder
{
    public record UpdateOrderCommand(Guid Id,
                                     string OrderName,
                                     AddressDto ShippingAddress,
                                     AddressDto BillingAddress,
                                     PaymentDto Payment) : ICommand<UpdateOrderResult>;

    public record UpdateOrderResult(bool IsSuccess);

    public class UpdateOrderCommandValidator : AbstractValidator<UpdateOrderCommand>
    {
        public UpdateOrderCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.OrderName)
                .NotEmpty()
                .WithMessage("OrderName is required.")
                .MaximumLength(200)
                .WithMessage("OrderName must not exceed 200 characters.");

            RuleFor(x => x.ShippingAddress)
                .NotNull()
                .WithMessage("ShippingAddress is required.");

            RuleFor(x => x.BillingAddress)
                .NotNull()
                .WithMessage("BillingAddress is required.");

            RuleFor(x => x.Payment)
                .NotNull()
                .WithMessage("Payment is required.");
        }
    }

    internal sealed class UpdateOrderCommandHandler(IOrderRepository repository) : ICommandHandler<UpdateOrderCommand, UpdateOrderResult>
    {
        public async Task<UpdateOrderResult> Handle(UpdateOrderCommand command, CancellationToken cancellationToken)
        {
            var order = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Order with id '{command.Id}' was not found.");

            order.Update(OrderName.Of(command.OrderName),
                         command.ShippingAddress.ToValueObject(),
                         command.BillingAddress.ToValueObject(),
                         command.Payment.ToValueObject());

            await repository.SaveChangesAsync(cancellationToken);

            return new UpdateOrderResult(true);
        }
    }
}
