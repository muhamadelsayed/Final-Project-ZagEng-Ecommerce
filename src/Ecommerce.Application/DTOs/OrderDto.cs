namespace Ecommerce.Application.DTOs;

public sealed record OrderDto(
    Guid Id,
    Guid? ClientId,
    string CustomerName,
    IReadOnlyList<OrderItemDto> Items,
    decimal Total,
    string ShippingAddress,
    string PaymentMethod,
    string Status,
    DateTime? CreatedAt
);

public sealed record OrderItemDto(
    Guid Id,
    Guid ProductId,
    string Title,
    string FeaturedImage,
    int Quantity,
    decimal UnitPrice
);