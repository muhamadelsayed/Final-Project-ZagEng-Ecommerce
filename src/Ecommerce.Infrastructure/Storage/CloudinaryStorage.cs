using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Ecommerce.Application.Interfaces;
using Microsoft.Extensions.Options;

namespace Ecommerce.Infrastructure.Storage;

public sealed class CloudinaryStorage : IImageStorage
{
    private readonly Cloudinary _cloudinary;
    private readonly CloudinaryOptions _options;

    public CloudinaryStorage(IOptions<CloudinaryOptions> options)
    {
        _options = options.Value;

        if (string.IsNullOrWhiteSpace(_options.CloudName) ||
            string.IsNullOrWhiteSpace(_options.ApiKey) ||
            string.IsNullOrWhiteSpace(_options.ApiSecret))
        {
            throw new InvalidOperationException(
                "Cloudinary settings are missing. Configure Cloudinary:CloudName, Cloudinary:ApiKey, and Cloudinary:ApiSecret.");
        }

        _cloudinary = new Cloudinary(new Account(
            _options.CloudName,
            _options.ApiKey,
            _options.ApiSecret));
    }

    public async Task<StoredImage> UploadAsync(
        Stream content,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default)
    {
        ArgumentNullException.ThrowIfNull(content);

        var uploadParameters = new ImageUploadParams
        {
            File = new FileDescription(fileName, content),
            Folder = _options.Folder,
            UseFilename = false,
            UniqueFilename = true
        };

        var result = await _cloudinary.UploadAsync(uploadParameters, cancellationToken);
        if (result.Error is not null)
        {
            throw new InvalidOperationException($"Cloudinary image upload failed: {result.Error.Message}");
        }

        return new StoredImage(result.PublicId, result.SecureUrl.ToString());
    }

    public async Task<bool> DeleteAsync(string publicId, CancellationToken cancellationToken = default)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(publicId);

        var result = await _cloudinary.DestroyAsync(
            new DeletionParams(publicId) { ResourceType = ResourceType.Image });

        if (result.Error is not null)
        {
            throw new InvalidOperationException($"Cloudinary image deletion failed: {result.Error.Message}");
        }

        return string.Equals(result.Result, "ok", StringComparison.OrdinalIgnoreCase) ||
                string.Equals(result.Result, "not found", StringComparison.OrdinalIgnoreCase);
    }
}
