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

    public async Task<List<WishlistItemDto>> GetWishlistByUserIdAsync(Guid userId)
    {
        return await _context.Wishlists
            .Where(w => w.UserId == userId)
            .Select(w => new WishlistItemDto
            {
                Id = w.Id,
                ProductId = w.ProductId,
                CreatedAt = w.CreatedAt
            })
            .ToListAsync();
    }

    public async Task<bool> AddToWishlistAsync(Guid userId, AddToWishlistRequestDto dto)
    {
        var exists = await _context.Wishlists
            .AnyAsync(w => w.UserId == userId && w.ProductId == dto.ProductId);

        if (exists) return true;

        var wishlistItem = new Wishlist
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            ProductId = dto.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        _context.Wishlists.Add(wishlistItem);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId)
    {
        var item = await _context.Wishlists
            .FirstOrDefaultAsync(w => w.UserId == userId && w.ProductId == productId);

        if (item == null) return false;

        _context.Wishlists.Remove(item);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ClearWishlistAsync(Guid userId)
    {
        var items = await _context.Wishlists
            .Where(w => w.UserId == userId)
            .ToListAsync();

        if (items.Count == 0) return false;

        _context.Wishlists.RemoveRange(items);
        return await _context.SaveChangesAsync() > 0;
    }
}