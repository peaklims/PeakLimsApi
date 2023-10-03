namespace PeakLims.Services.External;

using System.Net;
using Amazon.S3;
using Amazon.S3.Model;

public interface IFileStorage : IPeakLimsScopedService
{
    Task<string> UploadFileAsync(string bucketName, string key, Stream fileStream);
    Task<string> UploadFileAsync(string bucketName, string key, byte[] fileBytes);
    Task<string> UploadFileAsync(string bucketName, string key, string filePath);
    Task<string> UploadFileAsync(string bucketName, string key, string filePath, string contentType);
    Task<string> UploadFileAsync(string bucketName, string key, IFormFile formFile);
    Task<Stream> GetFileAsync(string bucketName, string key);
    string GetPreSignedUrl(string bucketName, string key, int durationInMinutes = 5);
    Task<bool> DeleteFileAsync(string bucketName, string key);
    Task<bool> DeleteFilesAsync(string bucketName, IEnumerable<string> keys);
}

public class FileStorage : IFileStorage
{
    private readonly IAmazonS3 _s3Client;

    public FileStorage(IAmazonS3 s3Client)
    {
        _s3Client = s3Client;
    }
    
    public async Task<string> UploadFileAsync(string bucketName, string key, Stream fileStream)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = fileStream
        };

        var response = await _s3Client.PutObjectAsync(putObjectRequest);

        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }
    
    public async Task<string> UploadFileAsync(string bucketName, string key, IFormFile formFile)
    {
        if (formFile == null)
        {
            throw new Exception("No file was provided for upload");
        }

        await UploadFileAsync(bucketName, key, formFile.OpenReadStream());

        return key;
    }
    
    public async Task<string> UploadFileAsync(string bucketName, string key, byte[] fileBytes)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            InputStream = new MemoryStream(fileBytes)
        };

        var response = await _s3Client.PutObjectAsync(putObjectRequest);

        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }
    
    public async Task<string> UploadFileAsync(string bucketName, string key, string filePath)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            FilePath = filePath
        };

        var response = await _s3Client.PutObjectAsync(putObjectRequest);

        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }
    
    public async Task<string> UploadFileAsync(string bucketName, string key, string filePath, string contentType)
    {
        var putObjectRequest = new PutObjectRequest
        {
            BucketName = bucketName,
            Key = key,
            FilePath = filePath,
            ContentType = contentType
        };

        var response = await _s3Client.PutObjectAsync(putObjectRequest);

        return response.HttpStatusCode == HttpStatusCode.OK ? key : null;
    }
    
    public async Task<Stream> GetFileAsync(string bucketName, string key)
    {
        var getObjectRequest = new GetObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await _s3Client.GetObjectAsync(getObjectRequest);

        return response.HttpStatusCode == HttpStatusCode.OK ? response.ResponseStream : null;
    }
    
    public string GetPreSignedUrl(string bucketName, string key, int durationInMinutes = 5)
    {
        var request = new GetPreSignedUrlRequest
        {
            BucketName = bucketName,
            Key = key,
            Expires = DateTime.Now.AddMinutes(durationInMinutes)
        };

        return _s3Client.GetPreSignedURL(request);
    }
    
    public async Task<bool> DeleteFileAsync(string bucketName, string key)
    {
        var deleteObjectRequest = new DeleteObjectRequest
        {
            BucketName = bucketName,
            Key = key
        };

        var response = await _s3Client.DeleteObjectAsync(deleteObjectRequest);

        return response.HttpStatusCode == HttpStatusCode.NoContent;
    }
    
    public async Task<bool> DeleteFilesAsync(string bucketName, IEnumerable<string> keys)
    {
        var deleteObjectsRequest = new DeleteObjectsRequest
        {
            BucketName = bucketName,
            Objects = keys.Select(key => new KeyVersion { Key = key }).ToList()
        };

        var response = await _s3Client.DeleteObjectsAsync(deleteObjectsRequest);

        return response.HttpStatusCode == HttpStatusCode.OK;
    }
}