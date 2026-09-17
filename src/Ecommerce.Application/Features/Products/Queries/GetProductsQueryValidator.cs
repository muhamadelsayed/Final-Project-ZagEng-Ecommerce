using FluentValidation;

namespace Ecommerce.Application.Features.Products.Queries;

public sealed class GetProductsQueryValidator : AbstractValidator<GetProductsQuery>
{
    private static readonly string[] AllowedSortFields = ["name", "price", "stock", "category"];
    private static readonly string[] AllowedDirections = ["asc", "desc"];

    public GetProductsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.SortBy)
            .Must(sortBy => string.IsNullOrWhiteSpace(sortBy) || AllowedSortFields.Contains(sortBy.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortBy must be name, price, stock, or category.");
        RuleFor(query => query.SortDirection)
            .Must(direction => string.IsNullOrWhiteSpace(direction) || AllowedDirections.Contains(direction.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be asc or desc.");
    }
}
