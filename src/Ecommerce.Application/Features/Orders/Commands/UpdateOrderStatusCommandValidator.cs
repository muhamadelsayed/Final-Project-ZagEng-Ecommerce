using FluentValidation;

namespace Ecommerce.Application.Features.Orders.Commands;

public sealed class UpdateOrderStatusCommandValidator : AbstractValidator<UpdateOrderStatusCommand>
{
    public UpdateOrderStatusCommandValidator()
    {
        RuleFor(x => x.OrderId).NotEmpty();
        RuleFor(x => x.NewStatus).NotEmpty();
    }
}