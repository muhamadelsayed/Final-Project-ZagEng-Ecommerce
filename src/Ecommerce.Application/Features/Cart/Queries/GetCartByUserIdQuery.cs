using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Cart.Queries;

public sealed record GetCartByUserIdQuery(Guid UserId) : IRequest<CartDto>;

public sealed class GetCartByUserIdQueryHandler : IRequestHandler<GetCartByUserIdQuery, CartDto>
{
    private readonly ICartRepository _cartRepository;

    public GetCartByUserIdQueryHandler(ICartRepository cartRepository)
    {
        _cartRepository = cartRepository;
    }

    public async Task<CartDto> Handle(GetCartByUserIdQuery request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetOrCreateByUserIdAsync(request.UserId, cancellationToken);

        var items = cart.CartItems.Select(ci => new CartItemDto(
            ci.Id,
            ci.ProductId,
            ci.Product.Title,
            ci.Product.Price,
            ci.Quantity ?? 1,
            ci.Product.FeaturedImage,
            ci.Product.Price * (ci.Quantity ?? 1)
        )).ToList();

        var totalAmount = items.Sum(i => i.SubTotal);

        return new CartDto(
            cart.Id,
            cart.UserId,
            items,
            totalAmount,
            cart.UpdatedAt
        );
    }
}