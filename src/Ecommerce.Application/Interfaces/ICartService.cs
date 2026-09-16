using Ecommerce.Application.DTOs.Cart;

namespace Ecommerce.Application.Interfaces;

public interface ICartService
{
    Task<CartDto> GetCartByUserIdAsync(Guid userId);
    Task<bool> AddItemToCartAsync(Guid userId, AddToCartRequestDto dto);
    Task<bool> RemoveItemFromCartAsync(Guid userId, Guid cartItemId);
    Task<bool> ClearCartAsync(Guid userId);
}