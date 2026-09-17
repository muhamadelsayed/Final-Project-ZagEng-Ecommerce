using Ecommerce.Domain.Entities;
using Ecommerce.Application.Interfaces;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _context;

    public CategoryRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Category>> GetAllAsync(CancellationToken cancellationToken = default)
        => await _context.Categories
            .AsNoTracking()
            .OrderBy(category => category.Name)
            .ToListAsync(cancellationToken);

    public Task<Category?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Categories.FirstOrDefaultAsync(category => category.Id == id, cancellationToken);

    public Task<bool> ExistsByNameAsync(string name, Guid? excludedCategoryId = null, CancellationToken cancellationToken = default)
    {
        var query = _context.Categories.Where(category => category.Name.ToLower() == name.ToLower());

        if (excludedCategoryId.HasValue)
        {
            query = query.Where(category => category.Id != excludedCategoryId.Value);
        }

        return query.AnyAsync(cancellationToken);
    }

    public async Task AddAsync(Category category, CancellationToken cancellationToken = default)
    {
        await _context.Categories.AddAsync(category, cancellationToken);
    }

    public void Remove(Category category)
        => _context.Categories.Remove(category);

    public async Task<bool> SaveChangesAsync(CancellationToken cancellationToken = default)
        => await _context.SaveChangesAsync(cancellationToken) > 0;
}
