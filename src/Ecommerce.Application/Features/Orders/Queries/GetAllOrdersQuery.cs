using Ecommerce.Application.DTOs;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Orders.Queries;

public sealed record GetAllOrdersQuery : IRequest<IReadOnlyList<OrderDto>>;

public sealed class GetAllOrdersQueryHandler : IRequestHandler<GetAllOrdersQuery, IReadOnlyList<OrderDto>>
{
    private readonly IOrderRepository _orderRepository;

    public GetAllOrdersQueryHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task<IReadOnlyList<OrderDto>> Handle(GetAllOrdersQuery request, CancellationToken cancellationToken)
    {
        var orders = await _orderRepository.GetAllAsync(cancellationToken);

        return orders.Select(o => new OrderDto(
            o.Id,
            o.UserId,
            o.User?.Name ?? "Customer",
            o.OrderItems.Select(oi => new OrderItemDto(
                oi.Id,
                oi.ProductId,
                oi.Product?.Title ?? "Product",
                oi.Product?.FeaturedImage ?? "",
                oi.Quantity,
                oi.UnitPrice
            )).ToList(),
            o.Total,
            o.ShippingAddress,
            o.PaymentMethod ?? "cash_on_delivery",
            o.Status ?? "pending",
            o.CreatedAt
        )).ToList();
    }
}