using FluentValidation;

namespace Ecommerce.Application.Features.Cart.Commands;

public sealed class UpdateCartItemQuantityCommandValidator : AbstractValidator<UpdateCartItemQuantityCommand>
{
    public UpdateCartItemQuantityCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
        RuleFor(x => x.Quantity).GreaterThan(0).WithMessage("Quantity must be at least 1.");
    }
}