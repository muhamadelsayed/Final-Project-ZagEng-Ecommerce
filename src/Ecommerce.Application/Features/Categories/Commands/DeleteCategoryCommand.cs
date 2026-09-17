using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.Common.Exceptions;
using MediatR;

namespace Ecommerce.Application.Features.Categories.Commands;

public sealed record DeleteCategoryCommand(Guid Id) : IRequest;

public sealed class DeleteCategoryCommandHandler : IRequestHandler<DeleteCategoryCommand>
{
    private readonly ICategoryRepository _categoryRepository;

    public DeleteCategoryCommandHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task Handle(DeleteCategoryCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException(nameof(Category), request.Id);
        }

        _categoryRepository.Remove(category);
        await _categoryRepository.SaveChangesAsync(cancellationToken);
    }
}
