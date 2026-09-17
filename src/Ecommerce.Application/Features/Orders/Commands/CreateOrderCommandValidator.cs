using FluentValidation;

namespace Ecommerce.Application.Features.Orders.Commands;

public sealed class CreateOrderCommandValidator : AbstractValidator<CreateOrderCommand>
{
    public CreateOrderCommandValidator()
    {
        RuleFor(x => x.UserId).NotEmpty();
        RuleFor(x => x.ShippingAddress).NotEmpty().MaximumLength(500);
        RuleFor(x => x.PaymentMethod).NotEmpty();
    }
}