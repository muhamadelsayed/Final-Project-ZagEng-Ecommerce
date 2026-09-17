using Ecommerce.Domain.Entities;

namespace Ecommerce.Application.Interfaces;

public interface ICartRepository
{
    Task<Cart?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task<Cart> GetOrCreateByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
    Task AddAsync(Cart cart, CancellationToken cancellationToken = default);
    Task AddItemAsync(CartItem cartItem, CancellationToken cancellationToken = default);
    void RemoveItem(CartItem cartItem);
    void ClearCart(Cart cart);
    Task SaveChangesAsync(CancellationToken cancellationToken = default);
}