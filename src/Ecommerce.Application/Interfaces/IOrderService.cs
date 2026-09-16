using Ecommerce.Application.DTOs.Order;

namespace Ecommerce.Application.Interfaces;

public interface IOrderService
{
    Task<IEnumerable<OrderDto>> GetUserOrdersAsync(Guid userId);
    Task<OrderDto?> GetOrderByIdAsync(Guid userId, Guid orderId);
    Task<OrderDto?> CreateOrderFromCartAsync(Guid userId, CreateOrderRequestDto dto);
}