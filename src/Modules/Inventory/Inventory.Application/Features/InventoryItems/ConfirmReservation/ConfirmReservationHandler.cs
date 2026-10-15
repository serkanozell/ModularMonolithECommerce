namespace Inventory.Application.Features.InventoryItems.ConfirmReservation
{
    public record ConfirmReservationCommand(Guid OrderId) : ICommand<ConfirmReservationResult>;

    public record ConfirmReservationResult(bool IsSuccess, string? FailureReason = null);

    public class ConfirmReservationCommandValidator : AbstractValidator<ConfirmReservationCommand>
    {
        public ConfirmReservationCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
        }
    }

    internal sealed class ConfirmReservationCommandHandler(IInventoryItemRepository repository) : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
    {
        public async Task<ConfirmReservationResult> Handle(ConfirmReservationCommand command, CancellationToken cancellationToken)
        {
            var items = await repository.GetByOrderIdWithReservationsAsync(command.OrderId, cancellationToken);

            if (items.Count == 0)
                return new ConfirmReservationResult(false, $"No active reservation found for order {command.OrderId}.");

            foreach (var item in items)
                if (!item.ConfirmReservation(command.OrderId))
                    return new ConfirmReservationResult(false, $"Reservation confirmation failed for order : {command.OrderId}, product : {item.ProductId}");

            try
            {
                await repository.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return new ConfirmReservationResult(false, "Stock changed during confirmation. Please retry.");
            }

            return new ConfirmReservationResult(true);
        }
    }
}
