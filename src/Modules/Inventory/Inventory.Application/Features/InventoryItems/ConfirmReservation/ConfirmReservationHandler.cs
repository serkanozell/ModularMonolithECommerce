namespace Inventory.Application.Features.InventoryItems.ConfirmReservation
{
    public record ConfirmReservationCommand(Guid Id, Guid OrderId) : ICommand<ConfirmReservationResult>;

    public record ConfirmReservationResult(bool IsSuccess);

    public class ConfirmReservationCommandValidator : AbstractValidator<ConfirmReservationCommand>
    {
        public ConfirmReservationCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.OrderId).NotEmpty();
        }
    }

    internal sealed class ConfirmReservationCommandHandler(IInventoryItemRepository repository) : ICommandHandler<ConfirmReservationCommand, ConfirmReservationResult>
    {
        public async Task<ConfirmReservationResult> Handle(ConfirmReservationCommand command, CancellationToken cancellationToken)
        {
            var item = await repository.GetByIdWithReservationsAsync(command.Id, command.OrderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Inventory item with id '{command.Id}' was not found.");

            if (!item.ConfirmReservation(command.OrderId))
                return new ConfirmReservationResult(false);

            try
            {
                await repository.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return new ConfirmReservationResult(false);
            }

            return new ConfirmReservationResult(true);
        }
    }
}
