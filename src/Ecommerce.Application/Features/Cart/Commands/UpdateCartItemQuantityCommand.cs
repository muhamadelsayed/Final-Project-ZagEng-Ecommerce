using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Features.Cart.Commands;

public sealed record UpdateCartItemQuantityCommand(Guid UserId, Guid ProductId, int Quantity) : IRequest;

public sealed class UpdateCartItemQuantityCommandHandler : IRequestHandler<UpdateCartItemQuantityCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public UpdateCartItemQuantityCommandHandler(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(UpdateCartItemQuantityCommand request, CancellationToken cancellationToken)
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

        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is not null && (product.Stock ?? 0) < request.Quantity)
        {
            throw new ConflictException("Requested quantity exceeds available stock.");
        }

        cartItem.Quantity = request.Quantity;
        cart.UpdatedAt = DateTime.UtcNow;

        await _cartRepository.SaveChangesAsync(cancellationToken);
    }
}