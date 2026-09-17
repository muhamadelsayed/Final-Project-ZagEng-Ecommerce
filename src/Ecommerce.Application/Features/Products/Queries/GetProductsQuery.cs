using Ecommerce.Application.Interfaces;
using Ecommerce.Application.DTOs;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries;

public sealed record GetProductsQuery(
    int Page = 1,
    int PageSize = 20,
    string? Search = null,
    string? SortBy = null,
    string? SortDirection = null) : IRequest<PagedResult<ProductListItemDto>>;

public sealed class GetProductsQueryHandler : IRequestHandler<GetProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IProductRepository _productRepository;

    public GetProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(
        GetProductsQuery request,
        CancellationToken cancellationToken)
    {
        var options = new ProductListOptions(
            request.Page,
            request.PageSize,
            request.Search,
            (request.SortBy ?? "name").Trim().ToLowerInvariant(),
            (request.SortDirection ?? "asc").Trim().ToLowerInvariant());

        var (products, totalCount) = await _productRepository.GetPagedAsync(options, cancellationToken);

        var items = products.Select(product => new ProductListItemDto(
            product.Id,
            product.Title,
            product.Price,
            product.Stock ?? 0,
            product.Category?.Name,
            product.FeaturedImage)).ToList();

        return new PagedResult<ProductListItemDto>(
            items,
            options.Page,
            options.PageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)options.PageSize),
            totalCount == 0 ? "No products found." : null);
    }
}
