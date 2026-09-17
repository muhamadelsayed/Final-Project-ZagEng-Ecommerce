using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public sealed class WishlistRepository : IWishlistRepository
{
    private readonly AppDbContext _context;

    public WishlistRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Wishlist>> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Wishlists
            .AsNoTracking()
            .Include(w => w.Product)
            .Where(w => w.UserId == userId)
            .OrderByDescending(w => w.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public Task<Wishlist?> GetByUserAndProductAsync(Guid userId, Guid productId, CancellationToken cancellationToken = default)
    {
        return _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId, cancellationToken);
    }

    public async Task AddAsync(Wishlist wishlist, CancellationToken cancellationToken = default)
    {
        await _context.Wishlists.AddAsync(wishlist, cancellationToken);
    }

    public void Remove(Wishlist wishlist)
    {
        _context.Wishlists.Remove(wishlist);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}