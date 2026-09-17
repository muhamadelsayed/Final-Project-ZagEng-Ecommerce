using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Infrastructure.Repositories;

public sealed class CartRepository : ICartRepository
{
    private readonly AppDbContext _context;

    public CartRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _context.Carts
            .Include(c => c.CartItems)
                .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId, cancellationToken);
    }

    public async Task<Cart> GetOrCreateByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cart = await GetByUserIdAsync(userId, cancellationToken);
        if (cart is null)
        {
            cart = new Cart
            {
                Id = Guid.NewGuid(),
                UserId = userId,
                UpdatedAt = DateTime.UtcNow
            };
            await _context.Carts.AddAsync(cart, cancellationToken);
            await _context.SaveChangesAsync(cancellationToken);
        }
        return cart;
    }

    public async Task AddAsync(Cart cart, CancellationToken cancellationToken = default)
    {
        await _context.Carts.AddAsync(cart, cancellationToken);
    }

    public async Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default)
    {
        await _context.CartItems.AddAsync(cartItem, cancellationToken);
    }

    public void RemoveItem(CartItem cartItem)
    {
        _context.CartItems.Remove(cartItem);
    }

    public void ClearCart(Cart cart)
    {
        _context.CartItems.RemoveRange(cart.CartItems);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        await _context.SaveChangesAsync(cancellationToken);
    }
}