using Ecommerce.Domain.Entities;
using Ecommerce.Application.Interfaces;
using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.DTOs;
using MediatR;

namespace Ecommerce.Application.Features.Categories.Commands;

public sealed record CreateCategoryCommand(string Name) : IRequest<CategoryDto>;

public sealed class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
{
    private readonly ICategoryRepository _categoryRepository;

    public CreateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<CategoryDto> Handle(CreateCategoryCommand request, CancellationToken cancellationToken)
    {
        var name = request.Name.Trim();
        if (await _categoryRepository.ExistsByNameAsync(name, cancellationToken: cancellationToken))
        {
            throw new ConflictException("A category with this name already exists.");
        }

        var category = new Category
        {
            Id = Guid.NewGuid(),
            Name = name,
            CreatedAt = DateTime.UtcNow
        };

        await _categoryRepository.AddAsync(category, cancellationToken);
        await _categoryRepository.SaveChangesAsync(cancellationToken);

        return new CategoryDto(category.Id, category.Name, category.CreatedAt);
    }
}
