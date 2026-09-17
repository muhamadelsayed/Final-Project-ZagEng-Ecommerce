using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed record DeleteProductCommand(Guid Id, bool Confirm) : IRequest;

public sealed class DeleteProductCommandHandler : IRequestHandler<DeleteProductCommand>
{
    private readonly IProductRepository _productRepository;

    public DeleteProductCommandHandler(IProductRepository productRepository)
    {
        _productRepository = productRepository;
    }

    public async Task Handle(DeleteProductCommand request, CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdAsync(request.Id, cancellationToken)
            ?? throw new NotFoundException(nameof(Product), request.Id);

        product.IsDeleted = true;
        product.DeletedAt = DateTime.UtcNow;
        product.UpdatedAt = DateTime.UtcNow;

        await _productRepository.SaveChangesAsync(cancellationToken);
    }
}
