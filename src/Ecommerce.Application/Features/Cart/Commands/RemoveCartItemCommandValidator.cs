using FluentValidation;

namespace Ecommerce.Application.Features.Cart.Commands;

public sealed class RemoveCartItemCommandValidator : AbstractValidator<RemoveCartItemCommand>
{
    public RemoveCartItemCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ProductId).NotEmpty();
    }
}