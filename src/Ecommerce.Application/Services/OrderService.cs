using Ecommerce.Application.DTOs.Order;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Ecommerce.Application.Services;

public class OrderService : IOrderService
{
    private readonly IAppDbContext _context;

    public OrderService(IAppDbContext context)
    {
        _context = context;
    }

    public async Task<OrderDto?> CreateOrderFromCartAsync(Guid userId, CreateOrderRequestDto dto)
    {
        var cart = await _context.Carts
            .Include(c => c.CartItems)
            .ThenInclude(ci => ci.Product)
            .FirstOrDefaultAsync(c => c.UserId == userId);

        if (cart == null || !cart.CartItems.Any())
            return null;

        var orderId = Guid.NewGuid();
        var order = new Order
        {
            Id = orderId,
            UserId = userId,
            ShippingAddress = dto.ShippingAddress,
            PaymentMethod = dto.PaymentMethod,
            Status = "Pending",
            CreatedAt = DateTime.UtcNow,
            Total = cart.CartItems.Sum(item => (item.Quantity ?? 1) * (item.Product?.Price ?? 0m)),
            OrderItems = cart.CartItems.Select(item => new OrderItem
            {
                Id = Guid.NewGuid(),
                OrderId = orderId,
                ProductId = item.ProductId,
                Quantity = item.Quantity ?? 1,
                UnitPrice = item.Product?.Price ?? 0m
            }).ToList()
        };

        _context.Orders.Add(order);
        _context.CartItems.RemoveRange(cart.CartItems);

        await _context.SaveChangesAsync();

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Total = order.Total,
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }

    public async Task<List<OrderDto>> GetOrdersByUserIdAsync(Guid userId)
    {
        return await _context.Orders
            .Where(o => o.UserId == userId)
            .Include(o => o.OrderItems)
            .OrderByDescending(o => o.CreatedAt)
            .Select(o => new OrderDto
            {
                Id = o.Id,
                UserId = o.UserId,
                Total = o.Total,
                ShippingAddress = o.ShippingAddress,
                PaymentMethod = o.PaymentMethod,
                Status = o.Status,
                CreatedAt = o.CreatedAt,
                Items = o.OrderItems.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    ProductId = i.ProductId,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice
                }).ToList()
            })
            .ToListAsync();
    }

    public async Task<OrderDto?> GetOrderByIdAsync(Guid userId, Guid orderId)
    {
        var order = await _context.Orders
            .Include(o => o.OrderItems)
            .FirstOrDefaultAsync(o => o.Id == orderId && o.UserId == userId);

        if (order == null) return null;

        return new OrderDto
        {
            Id = order.Id,
            UserId = order.UserId,
            Total = order.Total,
            ShippingAddress = order.ShippingAddress,
            PaymentMethod = order.PaymentMethod,
            Status = order.Status,
            CreatedAt = order.CreatedAt,
            Items = order.OrderItems.Select(i => new OrderItemDto
            {
                Id = i.Id,
                ProductId = i.ProductId,
                Quantity = i.Quantity,
                UnitPrice = i.UnitPrice
            }).ToList()
        };
    }
}