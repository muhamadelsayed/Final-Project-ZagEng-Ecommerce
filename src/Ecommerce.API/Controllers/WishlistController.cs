using System.Security.Claims;
using Ecommerce.Application.DTOs.Wishlist;
using Ecommerce.Application.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[Authorize]
public class WishlistController : ControllerBase
{
    private readonly IWishlistService _wishlistService;

    public WishlistController(IWishlistService wishlistService)
    {
        _wishlistService = wishlistService;
    }

    private Guid GetUserId()
    {
        var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
        return Guid.TryParse(userIdClaim, out var id) ? id : Guid.Empty;
    }

    [HttpGet]
    public async Task<IActionResult> GetWishlist()
    {
        var items = await _wishlistService.GetWishlistByUserIdAsync(GetUserId());
        return Ok(items);
    }

    [HttpPost]
    public async Task<IActionResult> AddToWishlist([FromBody] AddToWishlistRequestDto dto)
    {
        var success = await _wishlistService.AddToWishlistAsync(GetUserId(), dto);
        return success ? Ok(new { message = "Added to wishlist" }) : BadRequest();
    }

    [HttpDelete("{productId:guid}")]
    public async Task<IActionResult> RemoveFromWishlist(Guid productId)
    {
        var success = await _wishlistService.RemoveFromWishlistAsync(GetUserId(), productId);
        return success ? Ok(new { message = "Removed from wishlist" }) : NotFound();
    }

    [HttpDelete("clear")]
    public async Task<IActionResult> ClearWishlist()
    {
        var success = await _wishlistService.ClearWishlistAsync(GetUserId());
        return success ? Ok(new { message = "Wishlist cleared" }) : BadRequest();
    }
}