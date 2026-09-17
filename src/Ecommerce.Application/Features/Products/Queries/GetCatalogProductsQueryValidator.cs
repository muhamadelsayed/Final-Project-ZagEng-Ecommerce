using System.Data;
using FluentValidation;

namespace Ecommerce.Application.Features.Products.Queries;

public sealed class GetCatalogProductsQueryValidator : AbstractValidator<GetCatalogProductsQuery>
{
    private static readonly string[] AllowedDirections = ["asc", "desc"];

    public GetCatalogProductsQueryValidator()
    {
        RuleFor(query => query.Page).GreaterThan(0);
        RuleFor(query => query.PageSize).InclusiveBetween(1, 100);
        RuleFor(query => query.MinPrice).GreaterThanOrEqualTo(0).When(query => query.MinPrice.HasValue);
        RuleFor(query => query.MaxPrice).GreaterThanOrEqualTo(0).When(query => query.MaxPrice.HasValue);
        RuleFor(query => query.MaxPrice)
            .GreaterThanOrEqualTo(query => query.MinPrice)
            .When(query => query.MinPrice.HasValue && query.MaxPrice.HasValue)
            .WithMessage("MaxPrice must be greater than or equal to MinPrice.");
        RuleFor(query => query.Sort)
            .Must(direction => string.IsNullOrWhiteSpace(direction) || AllowedDirections.Contains(direction.Trim(), StringComparer.OrdinalIgnoreCase))
            .WithMessage("SortDirection must be asc or desc.");
    }
}
