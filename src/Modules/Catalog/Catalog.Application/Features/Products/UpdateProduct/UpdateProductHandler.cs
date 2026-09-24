namespace Catalog.Application.Features.Products.UpdateProduct
{
    public record UpdateProductCommand(Guid Id,
                                       string Name,
                                       List<string> Category,
                                       string? Description,
                                       decimal Price) : ICommand<UpdateProductResult>;

    public record UpdateProductResult(bool IsSuccess);

    public class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
    {
        public UpdateProductCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty()
                .WithMessage("Id is required.");

            RuleFor(x => x.Name)
                .NotEmpty()
                .WithMessage("Name is required.")
                .MaximumLength(200)
                .WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Category)
                .NotEmpty()
                .WithMessage("Category is required.");

            RuleFor(x => x.Description)
                .MaximumLength(2000)
                .WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0)
                .WithMessage("Price must be greater than zero.");
        }
    }

    internal sealed class UpdateProductCommandHandler(IProductRepository repository) : ICommandHandler<UpdateProductCommand, UpdateProductResult>
    {
        public async Task<UpdateProductResult> Handle(UpdateProductCommand command, CancellationToken cancellationToken)
        {
            var product = await repository.GetByIdAsync(command.Id, cancellationToken)
                ?? throw new KeyNotFoundException($"Product with id '{command.Id}' was not found.");

            if (await repository.ExistsByNameAsync(command.Name, command.Id, cancellationToken))
                throw new InvalidOperationException($"A product named '{command.Name}' already exists.");

            product.UpdateDetails(command.Name, command.Description);
            product.ChangeCategory(command.Category);
            product.ChangePrice(command.Price);

            repository.Update(product);

            await repository.SaveChangesAsync(cancellationToken);

            return new UpdateProductResult(true);
        }
    }
}