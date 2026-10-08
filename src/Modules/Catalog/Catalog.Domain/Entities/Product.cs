using Catalog.Domain.Events;

namespace Catalog.Domain.Entities
{
    public class Product : Aggregate<Guid>
    {
        public string Name { get; private set; } = default!;
        public List<string> Category { get; private set; } = new();
        public string? Description { get; private set; } = default!;
        public decimal Price { get; private set; }
        public int StockQuantity { get; private set; }
        public bool IsInStock { get; set; }

        private Product()
        {
        }

        private Product(string name, List<string> category, string? description, decimal price, int stockQuantity)
        {
            Id = Guid.NewGuid();
            Name = name;
            Category = category;
            Description = description;
            Price = price;
            StockQuantity = stockQuantity;
            IsActive = true;
            IsDeleted = false;
        }

        public static Product Create(string name, decimal price, int stockQuantity, List<string> category, string? description = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);
            ArgumentOutOfRangeException.ThrowIfNegative(price);
            ArgumentOutOfRangeException.ThrowIfNegative(stockQuantity);

            if (category == null || category.Count == 0)
                throw new ArgumentException("Category is required.", nameof(category));

            var product = new Product(name, category, description, price, stockQuantity);

            product.AddDomainEvent(new ProductCreatedEvent(product.Id,
                                                           product.Name,
                                                           product.Category,
                                                           product.Description,
                                                           product.Price,
                                                           product.StockQuantity));

            return product;
        }

        public void UpdateDetails(string name, string? description = null)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(name);

            Name = name;
            Description = description;
        }

        public void ChangePrice(decimal price)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(price);

            if (Price == price)
                return;

            var oldPrice = Price;
            Price = price;

            AddDomainEvent(new ProductPriceChangedEvent(Id, oldPrice, price));
        }

        public void ChangeCategory(List<string> category)
        {
            if (category == null || category.Count == 0)
                throw new ArgumentException("Category is required.", nameof(category));

            Category = category;
        }

        public void UpdateStockQuantity(bool isInStock, int quantity)
        {
            ArgumentOutOfRangeException.ThrowIfNegative(quantity);
            IsInStock = isInStock;
            StockQuantity = quantity;
        }

        public void Activate()
        {
            IsActive = true;
            IsDeleted = false;
        }

        public void Delete()
        {
            IsActive = false;
            IsDeleted = true;
        }
    }
}