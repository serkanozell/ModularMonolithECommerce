using Catalog.Domain.Entities;

namespace Catalog.Domain.Repositories
{
    public interface IProductRepository
    {
        void Add(Product product);

        void Update(Product product);

        Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

        Task<bool> ExistsByNameAsync(string name, Guid? excludedId = null, CancellationToken cancellationToken = default);

        Task<IReadOnlyList<Product>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

        Task<int> CountAsync(CancellationToken cancellationToken = default);

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}