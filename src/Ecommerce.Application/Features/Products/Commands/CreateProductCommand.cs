using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.Interfaces;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.DTOs; 
using MediatR;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed record ProductImageUpload(Stream Content, string FileName, string ContentType, long Length);

public sealed record CreateProductCommand(
    Guid CategoryId,
    string Title,
    string? Description,
    decimal Price,
    int Stock,
    bool IsVirtual,
    IReadOnlyList<ProductImageUpload> Images) : IRequest<ProductDto>;

public sealed class CreateProductCommandHandler : IRequestHandler<CreateProductCommand, ProductDto>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IProductRepository _productRepository;
    private readonly IImageStorage _imageStorage;

    public CreateProductCommandHandler(
        ICategoryRepository categoryRepository,
        IProductRepository productRepository,
        IImageStorage imageStorage)
    {
        _categoryRepository = categoryRepository;
        _productRepository = productRepository;
        _imageStorage = imageStorage;
    }

    public async Task<ProductDto> Handle(CreateProductCommand request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.CategoryId, cancellationToken);
        if (category is null)
        {
            throw new NotFoundException(nameof(Category), request.CategoryId);
        }

        var uploadedImages = new List<StoredImage>();

        try
        {
            foreach (var image in request.Images)
            {
                if (image.Content.CanSeek)
                {
                    image.Content.Position = 0;
                }

                uploadedImages.Add(await _imageStorage.UploadAsync(
                    image.Content,
                    image.FileName,
                    image.ContentType,
                    cancellationToken));
            }

            var product = new Product
            {
                Id = Guid.NewGuid(),
                CategoryId = request.CategoryId,
                Title = request.Title.Trim(),
                Description = string.IsNullOrWhiteSpace(request.Description)
                    ? null
                    : request.Description.Trim(),
                Price = request.Price,
                Stock = request.Stock,
                IsVirtual = request.IsVirtual ? (short)1 : (short)0,

                // First image = featured image
                FeaturedImage = uploadedImages[0].Url,

                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow,

                ProductMedia = uploadedImages.Select((image, index) => new ProductMedia
                {
                    Id = Guid.NewGuid(),
                    Url = image.Url,
                    PublicId = image.PublicId,
                    DisplayOrder = index
                }).ToList()
            };

            await _productRepository.AddAsync(product, cancellationToken);
            await _productRepository.SaveChangesAsync(cancellationToken);

            return new ProductDto(
                product.Id,
                product.CategoryId,
                product.Title,
                product.Description,
                product.Price,
                product.Stock ?? 0,
                product.IsVirtual == 1,
                product.FeaturedImage,
                product.ProductMedia.Select(image => image.Url).ToList());
        }
        catch
        {
            await DeleteUploadedImagesAsync(uploadedImages);
            throw;
        }
    }

    private async Task DeleteUploadedImagesAsync(IEnumerable<StoredImage> uploadedImages)
    {
        foreach (var image in uploadedImages)
        {
            try
            {
                await _imageStorage.DeleteAsync(image.PublicId);
            }
            catch
            {
                // Preserve the original upload or database exception. Failed cleanup can be retried separately.
            }
        }
    }
}
