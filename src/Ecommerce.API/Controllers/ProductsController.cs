using System.ComponentModel.DataAnnotations;
using Ecommerce.Application.Features.Products.Commands;
using Ecommerce.Application.Features.Products.Queries;
using Ecommerce.Application.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Metadata;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/products")]
[Tags("Products")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(PagedResult<ProductListItemDto>),
        StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] Guid? categoryId = null,
        [FromQuery] decimal? minPrice = null,
        [FromQuery] decimal? maxPrice = null,
        [FromQuery] string? sort = null,
        CancellationToken cancellationToken = default)
    {
        var products = await _sender.Send(
            new GetCatalogProductsQuery(
                page,
                pageSize,
                categoryId,
                minPrice,
                maxPrice,
                sort),
            cancellationToken);

        return Ok(products);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    [ProducesResponseType(
        typeof(ProductDetailsDto),
        StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailsDto>> GetById(
        Guid id,
        CancellationToken cancellationToken)
    {
        var product = await _sender.Send(
            new GetProductByIdQuery(id),
            cancellationToken);

        return Ok(product);
    }
}