namespace Catalog.Application.Features.Products.CreateProduct
{
    public record CreateProductCommand(string Name,
                                       List<string> Category,
                                       string? Description,
                                       decimal Price,
                                       int StockQuantity) : ICommand<CreateProductResult>;

    public record CreateProductResult(Guid Id);

    public class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
    {
        public CreateProductCommandValidator()
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(200).WithMessage("Name must not exceed 200 characters.");

            RuleFor(x => x.Category)
                .NotEmpty().WithMessage("Category is required.");

            RuleFor(x => x.Description)
                .MaximumLength(2000).WithMessage("Description must not exceed 2000 characters.");

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than zero.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("StockQuantity must not be negative.");
        }
    }

    internal sealed class CreateProductCommandHandler(IProductRepository repository) : ICommandHandler<CreateProductCommand, CreateProductResult>
    {
        public async Task<CreateProductResult> Handle(CreateProductCommand command, CancellationToken cancellationToken)
        {
            if (await repository.ExistsByNameAsync(command.Name, cancellationToken: cancellationToken))
                throw new InvalidOperationException($"A product named '{command.Name}' already exists.");

            var product = Product.Create(command.Name,
                                         command.Price,
                                         command.StockQuantity,
                                         command.Category,
                                         command.Description);

            repository.Add(product);

            await repository.SaveChangesAsync(cancellationToken);

            return new CreateProductResult(product.Id);
        }
    }
}