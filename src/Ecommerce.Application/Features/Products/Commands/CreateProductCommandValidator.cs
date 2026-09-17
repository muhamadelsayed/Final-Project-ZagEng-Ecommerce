using FluentValidation;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed class CreateProductCommandValidator : AbstractValidator<CreateProductCommand>
{
    private const long MaxImageSizeInBytes = 5 * 1024 * 1024;

    public CreateProductCommandValidator()
    {
        RuleFor(command => command.CategoryId).NotEmpty();
        RuleFor(command => command.Title).NotEmpty().MaximumLength(200);
        RuleFor(command => command.Description).MaximumLength(4_000);
        RuleFor(command => command.Price).GreaterThan(0).PrecisionScale(10, 2, false);
        RuleFor(command => command.Stock).GreaterThanOrEqualTo(0);

        RuleFor(command => command.Images)
            .NotEmpty()
            .Must(images => images.Count <= 5)
            .WithMessage("A product can contain at most five images.");

        RuleForEach(command => command.Images).ChildRules(image =>
        {
            image.RuleFor(upload => upload.FileName).NotEmpty();
            image.RuleFor(upload => upload.ContentType)
                .Must(contentType => contentType.StartsWith("image/", StringComparison.OrdinalIgnoreCase))
                .WithMessage("Only image files are allowed.");
            image.RuleFor(upload => upload.Length)
                .GreaterThan(0)
                .LessThanOrEqualTo(MaxImageSizeInBytes)
                .WithMessage("Each image must be 5 MB or smaller.");
        });
    }
}
