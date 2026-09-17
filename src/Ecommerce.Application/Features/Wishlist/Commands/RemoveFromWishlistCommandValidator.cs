using FluentValidation;

namespace Ecommerce.Application.Features.Wishlist.Commands;

public sealed class RemoveFromWishlistCommandValidator : AbstractValidator<RemoveFromWishlistCommand>
{
    public RemoveFromWishlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}