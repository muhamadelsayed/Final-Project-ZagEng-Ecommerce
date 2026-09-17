using FluentValidation;

namespace Ecommerce.Application.Features.Wishlist.Queries;

public sealed class GetWishlistQueryValidator : AbstractValidator<GetWishlistQuery>
{
    public GetWishlistQueryValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
    }
}