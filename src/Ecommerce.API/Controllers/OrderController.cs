using System.Security.Claims;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Features.Orders.Commands;
using Ecommerce.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/orders")]
[Tags("Orders")]
[Authorize]
public sealed class OrderController : ControllerBase
{
    private readonly ISender _sender;

    public OrderController(ISender sender)
    {
        _sender = sender;
    }

    private Guid GetUserId()
    {
        var claimValue = User.FindFirstValue(ClaimTypes.NameIdentifier) 
                         ?? User.FindFirstValue("sub");
        
        return Guid.TryParse(claimValue, out var userId) 
            ? userId 
            : throw new UnauthorizedAccessException("User ID claim is missing or invalid.");
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetUserOrders(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var orders = await _sender.Send(new GetUserOrdersQuery(userId), cancellationToken);
        return Ok(orders);
    }

    [HttpPost]
    [ProducesResponseType(typeof(OrderDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ActionResult<OrderDto>> CreateOrder([FromBody] CreateOrderRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var order = await _sender.Send(new CreateOrderCommand(userId, request.ShippingAddress, request.PaymentMethod), cancellationToken);
        return CreatedAtAction(nameof(GetUserOrders), new { id = order.Id }, order);
    }

    [HttpPost("{id:guid}/cancel")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> CancelOrder(Guid id, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _sender.Send(new CancelOrderCommand(id, userId), cancellationToken);
        return NoContent();
    }
}

public sealed class CreateOrderRequest
{
    public string ShippingAddress { get; init; } = string.Empty;
    public string PaymentMethod { get; init; } = "cash_on_delivery";
}