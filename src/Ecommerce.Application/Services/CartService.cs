using Ecommerce.Application.DTOs.Cart;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class CartService : ICartService
{
    private readonly IAppDbContext _context;

    public CartService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<CartDto> GetCartByUserIdAsync(Guid userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            return new CartDto { UserId = userId };
        }

        return new CartDto
        {
            Id = cart.Id,
            UserId = cart.UserId,
            Items = cart.CartItems.Select(i => new CartItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity ?? 0
            }).ToList()
        };
    }

    public async Task<bool> AddItemToCartAsync(Guid userId, AddToCartRequestDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null)
        {
            cart = new Cart
            {
                UserId = userId
            };
            _context.Carts.Add(cart);
        }

        var existingItem = cart.CartItems.FirstOrDefault(i => i.ProductId == dto.ProductId);
        if (existingItem != null)
        {
            existingItem.Quantity = (existingItem.Quantity ?? 0) + dto.Quantity;
        }
        else
        {
            cart.CartItems.Add(new CartItem
            {
                ProductId = dto.ProductId,
                Quantity = dto.Quantity
            });
        }

        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> RemoveItemFromCartAsync(Guid userId, Guid cartItemId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null) return false;

        var item = cart.CartItems.FirstOrDefault(i => i.Id == cartItemId);
        if (item == null) return false;

        cart.CartItems.Remove(item);
        return await _context.SaveChangesAsync() > 0;
    }

    public async Task<bool> ClearCartAsync(Guid userId)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || cart.CartItems.Count == 0) return false;

        cart.CartItems.Clear();
        return await _context.SaveChangesAsync() > 0;
    }
}