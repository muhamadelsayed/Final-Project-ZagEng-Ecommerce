using Ecommerce.Application.DTOs.Order;

namespace Ecommerce.Application.Interfaces;

public interface IOrderService
{
    Task<OrderDto?> CreateOrderFromCartAsync(Guid userId, CreateOrderRequestDto dto);
    Task<List<OrderDto>> GetOrdersByUserIdAsync(Guid userId);
    Task<OrderDto?> GetOrderByIdAsync(Guid userId, Guid orderId);
}