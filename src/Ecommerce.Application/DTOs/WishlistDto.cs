namespace Ecommerce.Application.DTOs;

public sealed record WishlistDto(
    Guid Id,
    Guid ProductId,
    string Title,
    decimal Price,
    string FeaturedImage,
    DateTime? CreatedAt
);