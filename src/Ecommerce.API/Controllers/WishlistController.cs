using System.Security.Claims;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Features.Wishlist.Commands;
using Ecommerce.Application.Features.Wishlist.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/wishlist")]
[Tags("Wishlist")]
[Authorize] // يتطلب تسجيل دخول
public sealed class WishlistController : ControllerBase
{
    private readonly ISender _sender;

    public WishlistController(ISender sender)
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
    [ProducesResponseType(typeof(IReadOnlyList<WishlistDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<WishlistDto>>> GetUserWishlist(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        var result = await _sender.Send(new GetWishlistQuery(userId), cancellationToken);
        return Ok(result);
    }

    [HttpPost("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddToWishlist(Guid productId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _sender.Send(new AddToWishlistCommand(userId, productId), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveFromWishlist(Guid productId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _sender.Send(new RemoveFromWishlistCommand(userId, productId), cancellationToken);
        return NoContent();
    }
}