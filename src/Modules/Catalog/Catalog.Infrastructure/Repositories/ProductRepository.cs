using Catalog.Domain.Repositories;
using Catalog.Infrastructure.Persistence;

namespace Catalog.Infrastructure.Repositories
{
    public class ProductRepository(CatalogDbContext context) : IProductRepository
    {
        public void Add(Product product) => context.Products.Add(product);

        public void Update(Product product) => context.Products.Update(product);

        public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default) =>
            context.Products.FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted, cancellationToken);

        public Task<bool> ExistsByNameAsync(string name, Guid? excludedId = null, CancellationToken cancellationToken = default) =>
            context.Products
                   .AsNoTracking()
                   .Where(p => !p.IsDeleted)
                   .AnyAsync(p => p.Name == name && (excludedId == null || p.Id != excludedId), cancellationToken);

        public async Task<IReadOnlyList<Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default) =>
            await ActiveProducts()
                  .AsNoTracking()
                  .OrderByDescending(p => p.CreatedAt)
                  .Skip((pageNumber - 1) * pageSize)
                  .Take(pageSize)
                  .ToListAsync(cancellationToken);

        public Task<int> CountAsync(CancellationToken cancellationToken = default) =>
            ActiveProducts().CountAsync(cancellationToken);

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            context.SaveChangesAsync(cancellationToken);

        private IQueryable<Product> ActiveProducts() =>
            context.Products.Where(p => !p.IsDeleted && p.IsActive);
    }
}