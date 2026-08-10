using BarberFlow.Application.Common.Exceptions;
using Microsoft.AspNetCore.Http;

namespace BarberFlow.Api.Validation
{
    public static class BarberShopImageValidator
    {
        private const long MaxImageSizeBytes = 1 * 1024 * 1024;
        private static readonly HashSet<string> AllowedExtensions = new(StringComparer.OrdinalIgnoreCase)
        {
            ".jpg", ".jpeg", ".png", ".webp"
        };

        private static readonly HashSet<string> AllowedContentTypes = new(StringComparer.OrdinalIgnoreCase)
        {
            "image/jpeg", "image/png", "image/webp"
        };

        public static async Task<byte[]> ReadAndValidateAsync(
            IFormFile file,
            string fieldName,
            CancellationToken cancellationToken)
        {
            if (file.Length == 0)
                throw new ValidationException($"{fieldName} es requerido.");

            if (file.Length > MaxImageSizeBytes)
                throw new ValidationException($"{fieldName} no puede superar 1 MB.");

            var extension = Path.GetExtension(file.FileName);

            if (!AllowedExtensions.Contains(extension) ||
                !AllowedContentTypes.Contains(file.ContentType))
            {
                throw new ValidationException(
                    $"{fieldName} debe ser una imagen JPG, JPEG, PNG o WebP.");
            }

            await using var stream = new MemoryStream();
            await file.CopyToAsync(stream, cancellationToken);
            var content = stream.ToArray();

            if (!HasExpectedSignature(content, extension))
            {
                throw new ValidationException(
                    $"{fieldName} no contiene una imagen válida.");
            }

            return content;
        }

        private static bool HasExpectedSignature(byte[] content, string extension)
        {
            return extension.ToLowerInvariant() switch
            {
                ".jpg" or ".jpeg" => content.Length >= 3 &&
                                     content[0] == 0xFF &&
                                     content[1] == 0xD8 &&
                                     content[2] == 0xFF,
                ".png" => content.Length >= 8 &&
                          content.AsSpan(0, 8).SequenceEqual(
                              new byte[] { 0x89, 0x50, 0x4E, 0x47, 0x0D, 0x0A, 0x1A, 0x0A }),
                ".webp" => content.Length >= 12 &&
                           content.AsSpan(0, 4).SequenceEqual("RIFF"u8) &&
                           content.AsSpan(8, 4).SequenceEqual("WEBP"u8),
                _ => false
            };
        }
    }
}
