using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist([FromQuery] Guid userId)
    {
        var items = await _wishlistService.GetUserWishlistAsync(userId);
        return Ok(items);
    }

    [HttpPost("{productId}")]
    public async Task<IActionResult> AddToWishlist([FromQuery] Guid userId, Guid productId)
    {
        var success = await _wishlistService.AddToWishlistAsync(userId, productId);
        if (!success)
            return BadRequest(new { message = "Item already in wishlist or could not be added." });

        return Ok(new { message = "Added to wishlist successfully." });
    }

    [HttpDelete("{productId}")]
    public async Task<IActionResult> RemoveFromWishlist([FromQuery] Guid userId, Guid productId)
    {
        var success = await _wishlistService.RemoveFromWishlistAsync(userId, productId);
        if (!success)
            return NotFound(new { message = "Item not found in wishlist." });

        return Ok(new { message = "Removed from wishlist successfully." });
    }
}