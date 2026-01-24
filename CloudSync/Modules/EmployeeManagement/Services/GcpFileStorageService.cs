using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Google.Cloud.Storage.V1;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudSync.Modules.EmployeeManagement.Services;

public class GcpFileStorageService : IFileStorageService
{
    private readonly ILogger<GcpFileStorageService> _logger;
    private readonly string _bucketName;
    private readonly StorageClient _storageClient;

    public GcpFileStorageService(
        ILogger<GcpFileStorageService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _bucketName = configuration["GCP:StorageBucketName"] 
            ?? throw new InvalidOperationException("GCP bucket name not configured");
            
        // In Cloud Run, this automatically uses the attached Service Account.
        // Locally, it looks for GOOGLE_APPLICATION_CREDENTIALS.
        _storageClient = StorageClient.Create();
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream, 
        string fileName, 
        string folderName, 
        string? contentType = null)
    {
        try 
        {
            // Sanitize filename to prevent issues
            var safeFileName = Path.GetFileName(fileName);
            var objectName = $"{folderName}/{Guid.NewGuid()}_{safeFileName}";
            
            await _storageClient.UploadObjectAsync(
                _bucketName,
                objectName,
                contentType ?? "application/octet-stream",
                fileStream);

            // Return the GCS URI. The Client will detect this and route it through the proxy.
            return $"https://storage.googleapis.com/{_bucketName}/{objectName}";
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading to GCS");
            throw;
        }
    }
    
    // This is the critical method for the Proxy solution
    public async Task<(Stream Stream, string ContentType)> GetFileAsync(string objectName)
    {
        try
        {
            var stream = new MemoryStream();
            // This bypasses public internet access and uses internal IAM permissions
            var obj = await _storageClient.DownloadObjectAsync(_bucketName, objectName, stream);
            stream.Position = 0;
            return (stream, obj.ContentType ?? "application/octet-stream");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error downloading from GCS: {ObjectName}", objectName);
            throw;
        }
    }

    public async Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl)) return false;

            // Extract object name from URL
            // URL format: https://storage.googleapis.com/BUCKET_NAME/OBJECT_NAME
            if (!fileUrl.Contains(_bucketName)) return false;

            var uri = new Uri(fileUrl);
            // The path comes in as /BUCKET_NAME/folder/file.jpg
            // We need just folder/file.jpg
            var path = uri.AbsolutePath.TrimStart('/');
            var objectName = path.Replace($"{_bucketName}/", "");

            await _storageClient.DeleteObjectAsync(_bucketName, objectName);
            return true;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting from GCS");
            return false;
        }
    }

    public Task<bool> FileExistsAsync(string fileUrl)
    {
        // Simplified check
        return Task.FromResult(!string.IsNullOrEmpty(fileUrl)); 
    }

    public string GetExtensionFromContentType(string contentType)
    {
        return contentType?.ToLowerInvariant() switch
        {
            "image/jpeg" or "image/jpg" => ".jpg",
            "image/png" => ".png",
            "application/pdf" => ".pdf",
            "application/msword" => ".doc",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" => ".docx",
            _ => ".bin"
        };
    }
}