using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Cart.Commands;

public sealed record RemoveCartItemCommand(Guid UserId, Guid ProductId) : IRequest;

public sealed class RemoveCartItemCommandHandler : IRequestHandler<RemoveCartItemCommand>
{
    private readonly ICartRepository _cartRepository;

    public RemoveCartItemCommandHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task Handle(RemoveCartItemCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart is null)
        {
            throw new NotFoundException("Cart", request.UserId);
        }

        var cartItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);
        if (cartItem is null)
        {
            throw new NotFoundException("CartItem", request.ProductId);
        }

        _cartRepository.RemoveItem(cartItem);
        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.SaveChangesAsync(cancellationToken);
    }
}