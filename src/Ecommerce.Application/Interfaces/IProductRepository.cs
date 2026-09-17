using Ecommerce.Domain.Entities;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Features.Products;

namespace Ecommerce.Application.Interfaces;

public interface IProductRepository
{
    Task<(IReadOnlyList<Product> Products, int TotalCount)> GetPagedAsync(
        ProductListOptions options,
        CancellationToken cancellationToken = default);
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);
    Task AddAsync(Product product, CancellationToken cancellationToken = default);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
    Task<Product?> GetByIdWithMediaAsync(
    Guid id,
    CancellationToken cancellationToken);
}

