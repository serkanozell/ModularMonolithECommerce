namespace Inventory.Application.Features.InventoryItems.ReleaseReserve
{
    public record ReleaseReserveCommand(Guid Id, Guid OrderId) : ICommand<ReleaseReserveResult>;

    public record ReleaseReserveResult(bool IsSuccess);

    public class ReleaseReserveCommandValidator : AbstractValidator<ReleaseReserveCommand>
    {
        public ReleaseReserveCommandValidator()
        {
            RuleFor(command => command.Id).NotEmpty();
            RuleFor(command => command.OrderId).NotEmpty();
        }
    }

    internal sealed class ReleaseReserveCommandHandler(IInventoryItemRepository repository) : ICommandHandler<ReleaseReserveCommand, ReleaseReserveResult>
    {
        public async Task<ReleaseReserveResult> Handle(ReleaseReserveCommand command, CancellationToken cancellationToken)
        {
            var item = await repository.GetByIdWithReservationsAsync(command.Id, command.OrderId, cancellationToken)
                ?? throw new KeyNotFoundException($"Inventory item with id '{command.Id}' was not found.");

            if (!item.ReleaseReservation(command.OrderId))
                return new ReleaseReserveResult(false);

            try
            {
                await repository.SaveChangesAsync(cancellationToken);
            }
            catch (Microsoft.EntityFrameworkCore.DbUpdateConcurrencyException)
            {
                return new ReleaseReserveResult(false);
            }

            return new ReleaseReserveResult(true);
        }
    }
}
