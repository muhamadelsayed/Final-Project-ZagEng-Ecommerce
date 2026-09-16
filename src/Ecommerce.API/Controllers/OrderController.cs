using Ecommerce.Application.DTOs.Order;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
        _orderService = orderService;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserOrders([FromQuery] Guid userId)
    {
        var orders = await _orderService.GetUserOrdersAsync(userId);
        return Ok(orders);
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrderById([FromQuery] Guid userId, Guid orderId)
    {
        var order = await _orderService.GetOrderByIdAsync(userId, orderId);
        if (order == null)
            return NotFound(new { message = "Order not found." });

        return Ok(order);
    }

    [HttpPost("checkout")]
    public async Task<IActionResult> Checkout([FromQuery] Guid userId, [FromBody] CreateOrderRequestDto dto)
    {
        var order = await _orderService.CreateOrderFromCartAsync(userId, dto);
        if (order == null)
            return BadRequest(new { message = "Cart is empty or could not process order." });

        return Ok(order);
    }
}