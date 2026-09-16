using Ecommerce.Application.DTOs.Wishlist;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class WishlistService : IWishlistService
{
    private readonly IAppDbContext _context;

    public WishlistService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(Guid userId)
    {
        return await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistResponseDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> AddToWishlistAsync(Guid userId, Guid productId)
    {
        var exists = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == productId);

        if (exists)
            return false;

        var wishlistItem = new Wishlist
        {
            UserId = userId,
            ProductId = productId
        };

        _context.Wishlists.Add(wishlistItem);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId)
    {
        var item = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item == null)
            return false;

        _context.Wishlists.Remove(item);
        return await _context.SaveChangesAsync() > 0;
    }
}