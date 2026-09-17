namespace Ecommerce.Application.Interfaces;

public interface IImageStorage
{
    Task<StoredImage> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);

    Task<bool> DeleteAsync(string publicId, CancellationToken cancellationToken = default);
}

public sealed record StoredImage(string PublicId, string Url);
