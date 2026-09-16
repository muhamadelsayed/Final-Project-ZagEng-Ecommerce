using Ecommerce.Application.DTOs.Wishlist;

namespace Ecommerce.Application.Interfaces;

public interface IWishlistService
{
    Task<IEnumerable<WishlistResponseDto>> GetUserWishlistAsync(Guid userId);
    Task<bool> AddToWishlistAsync(Guid userId, Guid productId);
    Task<bool> RemoveFromWishlistAsync(Guid userId, Guid productId);
}