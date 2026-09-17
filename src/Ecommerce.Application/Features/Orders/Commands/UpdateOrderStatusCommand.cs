using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Orders.Commands;

public sealed record UpdateOrderStatusCommand(Guid OrderId, string NewStatus) : IRequest;

public sealed class UpdateOrderStatusCommandHandler : IRequestHandler<UpdateOrderStatusCommand>
{
    private readonly IOrderRepository _orderRepository;
    private static readonly string[] AllowedStatuses = ["pending", "shipped", "delivered", "canceled"];

    public UpdateOrderStatusCommandHandler(IOrderRepository orderRepository)
    {
        _orderRepository = orderRepository;
    }

    public async Task Handle(UpdateOrderStatusCommand request, CancellationToken cancellationToken)
    {
        var status = request.NewStatus.Trim().ToLowerInvariant();
        if (!AllowedStatuses.Contains(status))
        {
            throw new ConflictException("Invalid order status.");
        }

        var order = await _orderRepository.GetByIdAsync(request.OrderId, cancellationToken);
        if (order is null)
        {
            throw new NotFoundException("Order", request.OrderId);
        }

        order.Status = status;
        await _orderRepository.SaveChangesAsync(cancellationToken);
    }
}