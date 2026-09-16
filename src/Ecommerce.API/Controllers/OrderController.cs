using System.Security.Claims;
using Ecommerce.Application.DTOs.Order;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromBody] CreateOrderRequestDto dto)
    {
        var order = await _orderService.CreateOrderFromCartAsync(GetUserId(), dto);
        if (order == null)
            return BadRequest(new { message = "Cart is empty or could not be processed." });

        return Ok(order);
    }

    [HttpGet]
    public async Task<IActionResult> GetMyOrders()
    {
        var orders = await _orderService.GetOrdersByUserIdAsync(GetUserId());
        return Ok(orders);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetOrderById(Guid id)
    {
        var order = await _orderService.GetOrderByIdAsync(GetUserId(), id);
        if (order == null)
            return NotFound();

        return Ok(order);
    }
}