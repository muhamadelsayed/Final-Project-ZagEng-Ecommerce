using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface IWishlistRepository
{
    Task<IReadOnlyList<Wishlist>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Wishlist?> GetByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default);
    Task AddAsync(Wishlist wishlist, CancellationToken cancellationToken = default);
    void Remove(Wishlist wishlist);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}