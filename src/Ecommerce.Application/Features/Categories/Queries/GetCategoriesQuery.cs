using Ecommerce.Domain.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Categories;

public sealed record GetCategoriesQuery : IRequest<IReadOnlyList<CategoryDto>>;

public sealed class GetCategoriesQueryHandler : IRequestHandler<GetCategoriesQuery, IReadOnlyList<CategoryDto>>
{
    private readonly ICategoryRepository _categoryRepository;

    public GetCategoriesQueryHandler(ICategoryRepository categoryRepository)
    {
        _categoryRepository = categoryRepository;
    }

    public async Task<IReadOnlyList<CategoryDto>> Handle(GetCategoriesQuery request, CancellationToken cancellationToken)
    {
        var categories = await _categoryRepository.GetAllAsync(cancellationToken);
        return categories
            .Select(category => new CategoryDto(category.Id, category.Name, category.CreatedAt))
            .ToList();
    }
}
