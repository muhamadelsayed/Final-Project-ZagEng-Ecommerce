using System.Security.Claims;
using Ecommerce.Application.DTOs;
using Ecommerce.Application.Features.Cart.Commands;
using Ecommerce.Application.Features.Cart.Queries;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/cart")]
[Tags("Cart")]
[Authorize]
public sealed class CartController : ControllerBase
{
    private readonly ISender _sender;

    public CartController(ISender sender)
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
    [ProducesResponseType(typeof(CartDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CartDto>> GetCart(CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        // تم ربطها بالـ Query الصحيحة GetCartByUserIdQuery
        var cart = await _sender.Send(new GetCartByUserIdQuery(userId), cancellationToken);
        return Ok(cart);
    }

    [HttpPost("items")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> AddItem([FromBody] AddCartItemRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _sender.Send(new AddToCartCommand(userId, request.ProductId, request.Quantity), cancellationToken);
        return NoContent();
    }

    [HttpPut("items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> UpdateItemQuantity(Guid productId, [FromBody] UpdateCartItemRequest request, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _sender.Send(new UpdateCartItemQuantityCommand(userId, productId, request.Quantity), cancellationToken);
        return NoContent();
    }

    [HttpDelete("items/{productId:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> RemoveItem(Guid productId, CancellationToken cancellationToken)
    {
        var userId = GetUserId();
        await _sender.Send(new RemoveCartItemCommand(userId, productId), cancellationToken);
        return NoContent();
    }
}

public sealed class AddCartItemRequest
{
    public Guid ProductId { get; init; }
    public int Quantity { get; init; } = 1;
}

public sealed class UpdateCartItemRequest
{
    public int Quantity { get; init; }
}