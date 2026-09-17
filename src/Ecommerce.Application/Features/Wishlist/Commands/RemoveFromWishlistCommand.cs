using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Wishlist.Commands;

public sealed record RemoveFromWishlistCommand(Guid UserId, Guid ProductId) : IRequest;

public sealed class RemoveFromWishlistCommandHandler : IRequestHandler<RemoveFromWishlistCommand>
{
    private readonly IWishlistRepository _wishlistRepository;

    public RemoveFromWishlistCommandHandler(IWishlistRepository wishlistRepository)
    {
        _wishlistRepository = wishlistRepository;
    }

    public async Task Handle(RemoveFromWishlistCommand request, CancellationToken cancellationToken)
    {
        var wishlist = await _wishlistRepository.GetByUserAndProductAsync(
            request.UserId, 
            request.ProductId, 
            cancellationToken);

        if (wishlist is null)
        {
            throw new NotFoundException(nameof(global::Ecommerce.Domain.Entities.Wishlist), request.ProductId);
        }

        _wishlistRepository.Remove(wishlist);
        await _wishlistRepository.SaveChangesAsync(cancellationToken);
    }
}