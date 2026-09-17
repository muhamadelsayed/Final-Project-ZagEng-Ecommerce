using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Features.Cart.Commands;

public sealed record AddToCartCommand(Guid UserId, Guid ProductId, int Quantity) : IRequest;

public sealed class AddToCartCommandHandler : IRequestHandler<AddToCartCommand>
{
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public AddToCartCommandHandler(ICartRepository cartRepository, IProductRepository productRepository)
    {
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(AddToCartCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException(nameof(Product), request.ProductId);
        }

        if ((product.Stock ?? 0) < request.Quantity)
        {
            throw new ConflictException("Requested quantity exceeds available stock.");
        }

        var cart = await _cartRepository.GetOrCreateByUserIdAsync(request.UserId, cancellationToken);

        var existingItem = cart.CartItems.FirstOrDefault(ci => ci.ProductId == request.ProductId);

        if (existingItem is not null)
        {
            var newQuantity = (existingItem.Quantity ?? 1) + request.Quantity;
            if ((product.Stock ?? 0) < newQuantity)
            {
                throw new ConflictException("Total quantity in cart exceeds available stock.");
            }
            existingItem.Quantity = newQuantity;
        }
        else
        {
            var cartItem = new CartItem
            {
                Id = Guid.NewGuid(),
                CartId = cart.Id,
                ProductId = request.ProductId,
                Quantity = request.Quantity
            };
            await _cartRepository.AddItemAsync(cartItem, cancellationToken);
        }

        cart.UpdatedAt = DateTime.UtcNow;
        await _cartRepository.SaveChangesAsync(cancellationToken);
    }
}