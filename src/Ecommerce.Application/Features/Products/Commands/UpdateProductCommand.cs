using Ecommerce.Application.Common.Exceptions;
using Ecommerce.Application.DTOs;
using Ecommerce.Domain.Entities;
using Ecommerce.Application.Interfaces;
using MediatR;

namespace Ecommerce.Application.Features.Products.Commands;

public sealed record UpdateProductCommand(
    Guid Id,
    Guid CategoryId,
    string Title,
    string? Description,
    decimal Price,
    int Stock,
    bool IsVirtual,
    IReadOnlyCollection<Guid> RemovedImageIds,
    IReadOnlyCollection<ProductImageUpload> Images
) : IRequest<ProductDetailsDto>;

public sealed class UpdateProductCommandHandler
    : IRequestHandler<UpdateProductCommand, ProductDetailsDto>
{
    private readonly IProductRepository _productRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IImageStorage _imageStorage;

    public UpdateProductCommandHandler(
        IProductRepository productRepository,
        ICategoryRepository categoryRepository,
        IImageStorage imageStorage)
    {
        _productRepository = productRepository;
        _categoryRepository = categoryRepository;
        _imageStorage = imageStorage;
    }

    public async Task<ProductDetailsDto> Handle(
    UpdateProductCommand request,
    CancellationToken cancellationToken)
    {
        var product = await _productRepository.GetByIdWithMediaAsync(
            request.Id,
            cancellationToken)
            ?? throw new NotFoundException(
                nameof(Product),
                request.Id);

        if (await _categoryRepository.GetByIdAsync(
                request.CategoryId,
                cancellationToken) is null)
        {
            throw new NotFoundException(
                nameof(Category),
                request.CategoryId);
        }

        // Keep track of newly uploaded images
        // so they can be deleted if the update fails.
        var uploadedImages = new List<StoredImage>();

        try
        {
            // Update product information
            product.CategoryId = request.CategoryId;
            product.Title = request.Title.Trim();
            product.Description = string.IsNullOrWhiteSpace(request.Description)
                ? null
                : request.Description.Trim();
            product.Price = request.Price;
            product.Stock = request.Stock;
            product.IsVirtual = request.IsVirtual ? (short)1 : (short)0;
            product.UpdatedAt = DateTime.UtcNow;

            // Remove existing images
            foreach (var imageId in request.RemovedImageIds)
            {
                var media = product.ProductMedia
                    .FirstOrDefault(x => x.Id == imageId);

                if (media is null)
                    continue;

                await _imageStorage.DeleteAsync(
                    media.PublicId,
                    cancellationToken);

                product.ProductMedia.Remove(media);
            }

            // Add new images
            foreach (var upload in request.Images)
            {
                if (upload.Content.CanSeek)
                {
                    upload.Content.Position = 0;
                }

                var result = await _imageStorage.UploadAsync(
                    upload.Content,
                    upload.FileName,
                    upload.ContentType,
                    cancellationToken);

                // Track the uploaded Cloudinary image
                uploadedImages.Add(result);

                product.ProductMedia.Add(new ProductMedia
                {
                    Id = Guid.NewGuid(),
                    ProductId = product.Id,
                    PublicId = result.PublicId,
                    Url = result.Url,
                    DisplayOrder = product.ProductMedia.Count
                });
            }

            // Recalculate image order
            var orderedMedia = product.ProductMedia
                .OrderBy(x => x.DisplayOrder)
                .ToList();

            for (var i = 0; i < orderedMedia.Count; i++)
            {
                orderedMedia[i].DisplayOrder = i;
            }

            // First image is always the featured image
            product.FeaturedImage = orderedMedia.Count > 0
                ? orderedMedia[0].Url
                : string.Empty;

            await _productRepository.SaveChangesAsync(
                cancellationToken);

            return ProductDetailsDto.From(product);
        }
        catch
        {
            // Database update failed after uploading new images.
            // Remove those newly uploaded images from Cloudinary.
            await DeleteUploadedImagesAsync(uploadedImages);

            throw;
        }
    }

    private async Task DeleteUploadedImagesAsync(
    IEnumerable<StoredImage> uploadedImages)
    {
        foreach (var image in uploadedImages)
        {
            try
            {
                await _imageStorage.DeleteAsync(image.PublicId);
            }
            catch
            {
                // Preserve the original exception.
                // Failed cleanup can be retried separately.
            }
        }
    }
}