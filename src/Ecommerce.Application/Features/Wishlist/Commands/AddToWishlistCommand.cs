using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Wishlist.Commands;

public sealed record AddToWishlistCommand(Guid UserId, Guid ProductId) : IRequest;

public sealed class AddToWishlistCommandHandler : IRequestHandler<AddToWishlistCommand>
{
    private readonly IWishlistRepository _wishlistRepository;
    private readonly IProductRepository _productRepository;

    public AddToWishlistCommandHandler(
        IWishlistRepository wishlistRepository,
        IProductRepository productRepository)
    {
        _wishlistRepository = wishlistRepository;
        _productRepository = productRepository;
    }

    public async Task Handle(AddToWishlistCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.ProductId, cancellationToken);
        if (product is null)
        {
            throw new NotFoundException(nameof(Ecommerce.Domain.Entities.Product), request.ProductId);
        }

        var existingWishlist = await _wishlistRepository.GetByUserAndProductAsync(
            request.UserId, 
            request.ProductId, 
            cancellationToken);

        if (existingWishlist is not null)
        {
            throw new ConflictException("Product is already in your wishlist.");
        }

        // استخدام المسار الكامل للـ Entity عشان منع التداخل مع الـ Namespace
        var wishlist = new global::Ecommerce.Domain.Entities.Wishlist
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            ProductId = request.ProductId,
            CreatedAt = DateTime.UtcNow
        };

        await _wishlistRepository.AddAsync(wishlist, cancellationToken);
        await _wishlistRepository.SaveChangesAsync(cancellationToken);
    }
}