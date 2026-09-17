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
// [Authorize(Roles = "admin")]
public sealed class ProductsController : ControllerBase
{
    private readonly ISender _sender;

    public ProductsController(ISender sender)
    {
        _sender = sender;
    }

    /// <summary>Gets active products for inventory monitoring.</summary>
    [HttpGet]
    [ProducesResponseType(typeof(PagedResult<ProductListItemDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<PagedResult<ProductListItemDto>>> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 20,
        [FromQuery] string? search = null,
        [FromQuery] string? sortBy = null,
        [FromQuery] string? sortDirection = null,
        CancellationToken cancellationToken = default)
    {
        var products = await _sender.Send(new GetProductsQuery(
            page,
            pageSize,
            search,
            sortBy,
            sortDirection), cancellationToken);

        return Ok(products);
    }

    /// <summary>Gets a product's current values for the admin edit form.</summary>
    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailsDto>> GetById(Guid id, CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetProductByIdQuery(id), cancellationToken));

    /// <summary>Creates a product and uploads its images to Cloudinary.</summary>
    /// <remarks>Send this endpoint as multipart/form-data. At least one image is required.</remarks>
    [HttpPost]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<ActionResult<ProductDto>> Create(
        [FromForm] CreateProductRequest request,
        CancellationToken cancellationToken)
    {
        var uploads = new List<ProductImageUpload>();

        try
        {
            foreach (var image in request.Images)
            {
                uploads.Add(new ProductImageUpload(
                    image.OpenReadStream(),
                    image.FileName,
                    image.ContentType,
                    image.Length));
            }

            var product = await _sender.Send(new CreateProductCommand(
                request.CategoryId,
                request.Title,
                request.Description,
                request.Price,
                request.Stock,
                request.IsVirtual,
                uploads), cancellationToken);

            return Created($"/api/products/{product.Id}", product);
        }
        finally
        {
            foreach (var upload in uploads)
            {
                await upload.Content.DisposeAsync();
            }
        }
    }

    /// <summary>Updates a product's details.</summary>
   /// <summary>Updates a product's details and images.</summary>
    /// <remarks>Send this endpoint as multipart/form-data.</remarks>
    [HttpPut("{id:guid}")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(typeof(ProductDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ProductDetailsDto>> Update(
        Guid id,
        [FromForm] UpdateProductRequest request,
        CancellationToken cancellationToken)
    {
        var uploads = new List<ProductImageUpload>();

        try
        {
            foreach (var image in request.Images)
            {
                uploads.Add(new ProductImageUpload(
                    image.OpenReadStream(),
                    image.FileName,
                    image.ContentType,
                    image.Length));
            }

            var product = await _sender.Send(
                new UpdateProductCommand(
                    id,
                    request.CategoryId,
                    request.Title,
                    request.Description,
                    request.Price,
                    request.Stock,
                    request.IsVirtual,
                    request.RemovedImageIds,
                    uploads),
                cancellationToken);

            return Ok(product);
        }
        finally
        {
            foreach (var upload in uploads)
            {
                await upload.Content.DisposeAsync();
            }
        }
    }

    /// <summary>Soft-deletes a product after explicit confirmation.</summary>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(
        Guid id,
        [FromQuery] bool confirm,
        CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteProductCommand(id, confirm), cancellationToken);
        return NoContent();
    }
}

public sealed class CreateProductRequest
{
    [Required]
    public Guid CategoryId { get; init; }

    [Required]
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    [Required]
    public decimal Price { get; init; }
    [Required]
    public int Stock { get; init; }
    [Required]
    public bool IsVirtual { get; init; }
    public List<IFormFile> Images { get; init; } = [];
}

public sealed class UpdateProductRequest
{
    public Guid CategoryId { get; init; }
    public string Title { get; init; } = string.Empty;
    public string? Description { get; init; }
    public decimal Price { get; init; }
    public int Stock { get; init; }
    public bool IsVirtual { get; init; }
    public List<Guid> RemovedImageIds { get; init; } = [];
    public List<IFormFile> Images { get; init; } = [];
}
