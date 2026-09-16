using System.ComponentModel.DataAnnotations;
using Ecommerce.Application.Features.Categories;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Ecommerce.API.Controllers;

[ApiController]
[Route("api/categories")]
[Authorize(Roles = "admin")]
public sealed class CategoriesController : ControllerBase
{
    private readonly ISender _sender;

    public CategoriesController(ISender sender)
    {
        _sender = sender;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IReadOnlyList<CategoryDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<IReadOnlyList<CategoryDto>>> GetAll(CancellationToken cancellationToken)
        => Ok(await _sender.Send(new GetCategoriesQuery(), cancellationToken));

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id, CancellationToken cancellationToken)
    {
        var category = await _sender.Send(new GetCategoryByIdQuery(id), cancellationToken);
        return Ok(category);
    }

    [HttpPost]
    [ProducesResponseType(typeof(CategoryDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<CategoryDto>> Create(CreateCategoryRequest request, CancellationToken cancellationToken)
    {
        var category = await _sender.Send(new CreateCategoryCommand(request.Name), cancellationToken);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Update(Guid id, UpdateCategoryRequest request, CancellationToken cancellationToken)
    {
        await _sender.Send(new UpdateCategoryCommand(id, request.Name), cancellationToken);
        return NoContent();
    }

    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        await _sender.Send(new DeleteCategoryCommand(id), cancellationToken);
        return NoContent();
    }
}

public sealed class CreateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;
}

public sealed class UpdateCategoryRequest
{
    [Required]
    [StringLength(100, MinimumLength = 1)]
    public string Name { get; init; } = string.Empty;
}
