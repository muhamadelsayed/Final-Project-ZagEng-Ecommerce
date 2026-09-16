using Ecommerce.Application.DTOs.Wishlist;

namespace Ecommerce.Application.Interfaces;

public interface IWishlistService
{
    Task<List<WishlistItemDto>> GetWishlistByUserIdAsync(Guid userId);
    Task<bool> AddToWishlistAsync(Guid userId, AddToWishlistRequestDto dto);
    Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId);
    Task<bool> ClearWishlistAsync(Guid userId);
}