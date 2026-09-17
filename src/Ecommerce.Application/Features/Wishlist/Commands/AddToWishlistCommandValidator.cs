using FluentValidation;

namespace Ecommerce.Application.Features.Wishlist.Commands;

public sealed class AddToWishlistCommandValidator : AbstractValidator<AddToWishlistCommand>
{
    public AddToWishlistCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}