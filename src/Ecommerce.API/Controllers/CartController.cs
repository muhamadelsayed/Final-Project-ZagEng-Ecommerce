using Ecommerce.Application.DTOs.Cart;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CartController : ControllerBase
{
    private readonly ICartService _cartService;

    public CartController(ICartService cartService)
    {
        _cartService = cartService;
    }

    [HttpGet]
    public async Task<IActionResult> GetCart([FromQuery] Guid userId)
    {
        var cart = await _cartService.GetCartByUserIdAsync(userId);
        return Ok(cart);
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddItemToCart([FromQuery] Guid userId, [FromBody] AddToCartRequestDto dto)
    {
        var success = await _cartService.AddItemToCartAsync(userId, dto);
        if (!success)
            return BadRequest(new { message = "Could not add item to cart." });

        return Ok(new { message = "Item added to cart successfully." });
    }

    [HttpDelete("items/{cartItemId}")]
    public async Task<IActionResult> RemoveItem([FromQuery] Guid userId, Guid cartItemId)
    {
        var success = await _cartService.RemoveItemFromCartAsync(userId, cartItemId);
        if (!success)
            return NotFound(new { message = "Item not found in cart." });

        return Ok(new { message = "Item removed successfully." });
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearCart([FromQuery] Guid userId)
    {
        var success = await _cartService.ClearCartAsync(userId);
        if (!success)
            return BadRequest(new { message = "Cart is already empty or could not be cleared." });

        return Ok(new { message = "Cart cleared successfully." });
    }
}