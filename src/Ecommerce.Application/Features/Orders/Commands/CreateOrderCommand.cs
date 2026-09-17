using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using MediatR;

namespace Ecommerce.Application.Features.Orders.Commands;

public sealed record CreateOrderCommand(
    Guid UserId,
    string ShippingAddress,
    string PaymentMethod
) : IRequest<OrderDto>;

public sealed class CreateOrderCommandHandler : IRequestHandler<CreateOrderCommand, OrderDto>
{
    private readonly IOrderRepository _orderRepository;
    private readonly ICartRepository _cartRepository;
    private readonly IProductRepository _productRepository;

    public CreateOrderCommandHandler(
        IOrderRepository orderRepository,
        ICartRepository cartRepository,
        IProductRepository productRepository)
    {
        _orderRepository = orderRepository;
        _cartRepository = cartRepository;
        _productRepository = productRepository;
    }

    public async Task<OrderDto> Handle(CreateOrderCommand request, CancellationToken cancellationToken)
    {
        var cart = await _cartRepository.GetByUserIdAsync(request.UserId, cancellationToken);
        if (cart is null || !cart.CartItems.Any())
        {
            throw new ConflictException("Your cart is empty.");
        }

        decimal total = 0;
        var orderItems = new List<OrderItem>();

        foreach (var cartItem in cart.CartItems)
        {
            var product = cartItem.Product;
            int qty = cartItem.Quantity ?? 1;

            if ((product.Stock ?? 0) < qty)
            {
                throw new ConflictException($"Product '{product.Title}' is out of stock or requested quantity is not available.");
            }

            // خصم المخزون
            product.Stock -= qty;

            var unitPrice = product.Price;
            total += unitPrice * qty;

            orderItems.Add(new OrderItem
            {
                Id = Guid.NewGuid(),
                ProductId = product.Id,
                Quantity = qty,
                UnitPrice = unitPrice
            });
        }

        var order = new Order
        {
            Id = Guid.NewGuid(),
            UserId = request.UserId,
            Total = total,
            ShippingAddress = request.ShippingAddress,
            PaymentMethod = request.PaymentMethod,
            Status = "pending",
            CreatedAt = DateTime.UtcNow,
            OrderItems = orderItems
        };

        await _orderRepository.AddAsync(order, cancellationToken);

        // تفريغ السلة بعد الطلب
        _cartRepository.ClearCart(cart);

        await _orderRepository.SaveChangesAsync(cancellationToken);

        return new OrderDto(
            order.Id,
            order.UserId,
            order.User?.Name ?? "Customer",
            order.OrderItems.Select(oi => new OrderItemDto(
                oi.Id,
                oi.ProductId,
                oi.Product?.Title ?? "Product",
                oi.Product?.FeaturedImage ?? "",
                oi.Quantity,
                oi.UnitPrice
            )).ToList(),
            order.Total,
            order.ShippingAddress,
            order.PaymentMethod ?? "cash_on_delivery",
            order.Status ?? "pending",
            order.CreatedAt
        );
    }
}