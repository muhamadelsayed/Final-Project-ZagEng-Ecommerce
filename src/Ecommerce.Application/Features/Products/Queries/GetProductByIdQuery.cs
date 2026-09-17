using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.DTOs;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Products.Queries;

public sealed record GetProductByIdQuery(Guid Id) : IRequest<ProductDetailsDto>;

public sealed class GetProductByIdQueryHandler : IRequestHandler<GetProductByIdQuery, ProductDetailsDto>
{
    private readonly IProductRepository _productRepository;

    public GetProductByIdQueryHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task<ProductDetailsDto> Handle(GetProductByIdQuery request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        return ProductDetailsDto.From(product);
    }
}


