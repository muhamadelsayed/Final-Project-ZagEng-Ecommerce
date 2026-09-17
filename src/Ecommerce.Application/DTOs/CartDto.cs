namespace Ecommerce.Application.DTOs;

public sealed record CartDto(
    Guid Id,
    Guid UserId,
    IReadOnlyList<CartItemDto> Items,
    decimal TotalAmount,
    DateTime? UpdatedAt
);

public sealed record CartItemDto(
    Guid Id,
    Guid ProductId,
    string Title,
    decimal UnitPrice,
    int Quantity,
    string FeaturedImage,
    decimal SubTotal
);