using Ecommerce.Domain.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.Common.Exceptions;
using MediatR;

namespace Ecommerce.Application.Features.Categories;

public sealed record UpdateCategoryCommand(Guid Id, string Name) : IRequest;

public sealed class UpdateCategoryCommandHandler : IRequestHandler<UpdateCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public UpdateCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(UpdateCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }

        var name = request.Name.Trim();
        if (await _categoryRepository.ExistsByNameAsync(name, category.Id, cancellationToken))
        {
            throw new ConflictException("A category with this name already exists.");
        }

        category.Name = name;
        await _categoryRepository.SaveChangesAsync(cancellationToken);
    }
}
