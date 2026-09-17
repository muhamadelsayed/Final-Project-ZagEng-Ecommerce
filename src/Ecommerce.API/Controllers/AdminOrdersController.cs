using Ecommerce.Application.DTOs;
using Ecommerce.Application.Features.Orders.Commands;
using Ecommerce.Application.Features.Orders.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/admin/orders")]
[Tags("Admin Orders")]
[Authorize(Roles = "admin")] // مخصص للأدمن فقط
public sealed class AdminOrdersController : ControllerBase
{
    private readonly ISender _sender;

    public AdminOrdersController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<OrderDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<OrderDto>>> GetAllOrders(CancellationToken cancellationToken)
    {
        var orders = await _sender.Send(new GetAllOrdersQuery(), cancellationToken);
        return Ok(orders);
    }

    [HttpPut("{id:guid}/status")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateStatus(
        Guid id, 
        [FromBody] UpdateOrderStatusRequest request, 
        CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateOrderStatusCommand(id, request.Status), cancellationToken);
        return NoContent();
    }
}

public sealed class UpdateOrderStatusRequest
{
    public string Status { get; init; } = string.Empty;
}