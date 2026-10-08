namespace Catalog.Application.Features.Products
{
    public static class ProductMappings
    {
        public static ProductDto ToDto(this Product product) =>
            new(product.Id,
                product.Name,
                product.Category,
                product.Description,
                product.Price,
                product.StockQuantity,
                product.IsInStock,
                product.IsActive,
                product.CreatedAt,
                product.UpdatedAt);

        public static List<ProductDto> ToDtoList(this IEnumerable<Product> products) =>
            products.Select(p => p.ToDto()).ToList();
    }
}