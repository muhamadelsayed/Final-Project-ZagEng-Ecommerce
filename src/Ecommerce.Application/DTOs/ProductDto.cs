using Ecommerce.Domain.Entities;
namespace Ecommerce.Application.DTOs;

public sealed record ProductDto(
    Guid Id,
    Guid? CategoryId,
    string Title,
    string? Description,
    decimal Price,
    int Stock,
    bool IsVirtual,
    string FeaturedImage,
    IReadOnlyList<string> Images);

public sealed record ProductDetailsDto(
    Guid Id,
    Guid? CategoryId,
    string Title,
    string? Description,
    decimal Price,
    int Stock,
    bool IsVirtual,
    string FeaturedImage,
    IReadOnlyList<ProductImageDto> Images)
{
    public static ProductDetailsDto From(Product product) => new(
        product.Id,
        product.CategoryId,
        product.Title,
        product.Description,
        product.Price,
        product.Stock ?? 0,
        product.IsVirtual == 1,
        product.FeaturedImage,
        product.ProductMedia.Select(image => new ProductImageDto(image.Id, image.Url)).ToList());
}

public sealed record ProductImageDto(Guid Id, string Url);

public sealed record ProductListOptions(
    int Page,
    int PageSize,
    string? Search,
    string SortBy,
    string SortDirection);

public sealed record ProductListItemDto(
    Guid Id,
    string Name,
    decimal Price,
    int Stock,
    string? Category);

public sealed record PagedResult<T>(
    IReadOnlyList<T> Items,
    int Page,
    int PageSize,
    int TotalCount,
    int TotalPages,
    string? Message);