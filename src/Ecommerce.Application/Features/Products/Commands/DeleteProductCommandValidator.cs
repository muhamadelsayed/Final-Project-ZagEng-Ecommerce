using FluentValidation;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed class DeleteProductCommandValidator : AbstractValidator<DeleteProductCommand>
{
    public DeleteProductCommandValidator()
    {
        RuleFor(command => command.Id).NotEmpty();
        RuleFor(command => command.Confirm)
            .Equal(true)
            .WithMessage("Deletion requires confirm=true.");
    }
}
