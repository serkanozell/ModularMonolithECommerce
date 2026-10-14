namespace Inventory.Application.Features.InventoryItems.ReleaseReserve
{
    public record ReleaseReserveCommand(Guid OrderId) : ICommand<ReleaseReserveResult>;

    public record ReleaseReserveResult(bool IsSuccess);

    public class ReleaseReserveCommandValidator : AbstractValidator<ReleaseReserveCommand>
    {
        public ReleaseReserveCommandValidator()
        {
            RuleFor(command => command.OrderId).NotEmpty();
        }
    }

    internal sealed class ReleaseReserveCommandHandler(IInventoryItemRepository repository) : ICommandHandler<ReleaseReserveCommand, ReleaseReserveResult>
    {
        public async Task<ReleaseReserveResult> Handle(ReleaseReserveCommand command, CancellationToken cancellationToken)
        {
            var items = await repository.GetByOrderIdWithReservationsAsync(command.OrderId, cancellationToken);

            foreach (var item in items)
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