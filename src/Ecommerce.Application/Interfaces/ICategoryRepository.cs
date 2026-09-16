using Ecommerce.Domain.Entities;

namespace Ecommerce.Domain.Interfaces;

public interface ICategoryRepository
{
    Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task<bool> ExistsByNameAsync(string name, Guid? excludedCategoryId = null, CancellationToken cancellationToken = default);
    Task AddAsync(Category category, CancellationToken cancellationToken = default);
    void Remove(Category category);
    Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default);
}
