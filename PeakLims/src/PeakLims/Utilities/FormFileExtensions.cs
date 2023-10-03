namespace PeakLims.Utilities;

using System.Security.Cryptography;

public static class FormFileExtensions
{
    public static string GetFileHash(this IFormFile formFile)
    {
        using var sha256 = SHA256.Create();
        using var stream = formFile.OpenReadStream();
        var hash = sha256.ComputeHash(stream);
        return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
    }
}