using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Orders.Commands;

public sealed record CancelOrderCommand(Guid OrderId, Guid UserId) : IRequest;

public sealed class CancelOrderCommandHandler : IRequestHandler<CancelOrderCommand>
{
    private readonly IOrderRepository _orderRepository;

    public CancelOrderCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(CancelOrderCommand request, CancellationToken cancellationToken)
    {
        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null || order.UserId != request.UserId)
        {
            throw new NotFoundException("Order", request.OrderId);
        }

        if (!string.Equals(order.Status, "pending", StringComparison.OrdinalIgnoreCase))
        {
            throw new ConflictException("Only pending orders can be canceled.");
        }

        order.Status = "canceled";
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }
}