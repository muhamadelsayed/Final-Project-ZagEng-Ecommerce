using FluentValidation;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed class UpdateProductCommandValidator : AbstractValidator<UpdateProductCommand>
{
    public UpdateProductCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.CategoryId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Description).MaximumLength(4_000);
        RuleFor(command => command.Price).GreaterThanOrEqualTo(0).PrecisionScale(10, 2, false);
        RuleFor(command => command.Stock).GreaterThanOrEqualTo(0);
    }
}
