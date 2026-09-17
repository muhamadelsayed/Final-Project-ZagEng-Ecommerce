using Ecommerce.Domain.Entities;
using Ecommerce.Application.Interfaces;
using Ecommerce.Infrastructure.Data;
using Ecommerce.Application.DTOs;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public sealed class ProductRepository : IProductRepository
{
    private readonly AppDbContext _context;

    public ProductRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(Product product, CancellationToken cancellationToken = default)
    {
        await _context.Products.AddAsync(product, cancellationToken);
    }

    public Task SaveChangesAsync(CancellationToken cancellationToken = default)
        => _context.SaveChangesAsync(cancellationToken);

    public async Task<(IReadOnlyList<Product> Products, int TotalCount)> GetPagedAsync(
        ProductListOptions options,
        CancellationToken cancellationToken = default)
    {
        IQueryable<Product> query = _context.Products
            .AsNoTracking()
            .Include(product => product.Category)
            .Where(product => !product.IsDeleted);

        if (!string.IsNullOrWhiteSpace(options.Search))
        {
            var search = options.Search.Trim().ToLower();
            query = query.Where(product =>
                product.Title.ToLower().Contains(search) ||
                (product.Description != null && product.Description.ToLower().Contains(search)));
        }

        query = (options.SortBy, options.SortDirection) switch
        {
            ("price", "desc") => query.OrderByDescending(product => product.Price),
            ("price", _) => query.OrderBy(product => product.Price),
            ("stock", "desc") => query.OrderByDescending(product => product.Stock),
            ("stock", _) => query.OrderBy(product => product.Stock),
            ("category", "desc") => query.OrderByDescending(product => product.Category!.Name),
            ("category", _) => query.OrderBy(product => product.Category!.Name),
            ("name", "desc") => query.OrderByDescending(product => product.Title),
            _ => query.OrderBy(product => product.Title)
        };

        var totalCount = await query.CountAsync(cancellationToken);
        var products = await query
            .Skip((options.Page - 1) * options.PageSize)
            .Take(options.PageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }

    public Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        => _context.Products
            .Include(product => product.Category)
            .Include(product => product.ProductMedia)
            .FirstOrDefaultAsync(product => product.Id == id && !product.IsDeleted, cancellationToken);

    public async Task<Product?> GetByIdWithMediaAsync(
    Guid id,
    CancellationToken cancellationToken)
    {
        return await _context.Products
            .Include(p => p.ProductMedia)
            .FirstOrDefaultAsync(
                p => p.Id == id,
                cancellationToken);
    }

    public async Task<(IReadOnlyList<Product> Products, int TotalCount)>
        GetCatalogPagedAsync(
            int page,
            int pageSize,
            Guid? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sort,
            CancellationToken cancellationToken)
    {
        IQueryable<Product> query = _context.Products
            .AsNoTracking()
            .Where(p => !p.IsDeleted);

        if (categoryId.HasValue)
        {
            query = query.Where(p => p.CategoryId == categoryId);
        }

        if (minPrice.HasValue)
        {
            query = query.Where(p => p.Price >= minPrice.Value);
        }

        if (maxPrice.HasValue)
        {
            query = query.Where(p => p.Price <= maxPrice.Value);
        }

        query = sort switch
        {
            "desc" => query.OrderByDescending(p => p.Price),
            _ => query.OrderBy(p => p.Price)
        };

        var totalCount = await query.CountAsync(cancellationToken);

        var products = await query
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (products, totalCount);
    }
}
