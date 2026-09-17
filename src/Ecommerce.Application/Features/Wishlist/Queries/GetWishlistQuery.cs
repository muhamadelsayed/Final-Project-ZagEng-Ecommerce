using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Wishlist.Queries;

public sealed record GetWishlistQuery(Guid UserId) : IRequest<IReadOnlyList<WishlistDto>>;

public sealed class GetWishlistQueryHandler : IRequestHandler<GetWishlistQuery, IReadOnlyList<WishlistDto>>
{
    private readonly IWishlistRepository _wishlistRepository;

    public GetWishlistQueryHandler(IWishlistRepository wishlistRepository)
    {
        _wishlistRepository = wishlistRepository;
    }

    public async Task<IReadOnlyList<WishlistDto>> Handle(GetWishlistQuery request, CancellationToken cancellationToken)
    {
        var wishlists = await _wishlistRepository.GetByUserIdAsync(request.UserId, cancellationToken);

        return wishlists.Select(w => new WishlistDto(
            w.Id,
            w.ProductId,
            w.Product.Title,
            w.Product.Price,
            w.Product.FeaturedImage,
            w.CreatedAt
        )).ToList();
    }
}