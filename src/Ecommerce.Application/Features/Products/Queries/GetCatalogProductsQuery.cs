using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries;

public sealed record GetCatalogProductsQuery(
    int Page,
    int PageSize,
    Guid? CategoryId,
    decimal? MinPrice,
    decimal? MaxPrice,
    string? Sort
) : IRequest<PagedResult<ProductListItemDto>>;

public sealed class GetCatalogProductsQueryHandler : IRequestHandler<GetCatalogProductsQuery, PagedResult<ProductListItemDto>>
{
    private readonly IProductRepository _productRepository;

    public GetCatalogProductsQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<PagedResult<ProductListItemDto>> Handle(
    GetCatalogProductsQuery request,
    CancellationToken cancellationToken)
    {
        var (products, totalCount) = await _productRepository.GetCatalogPagedAsync(
            request.Page,
            request.PageSize,
            request.CategoryId,
            request.MinPrice,
            request.MaxPrice,
            request.Sort,
            cancellationToken);

        var items = products.Select(product => new ProductListItemDto(
            product.Id,
            product.Title,
            product.Price,
            product.Stock ?? 0,
            product.Category?.Name)).ToList();

        return new PagedResult<ProductListItemDto>(
            items,
            request.Page,
            request.PageSize,
            totalCount,
            (int)Math.Ceiling(totalCount / (double)request.PageSize),
            totalCount == 0 ? "No products found." : null);
    }
}


