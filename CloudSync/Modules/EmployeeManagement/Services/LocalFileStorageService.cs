using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace CloudSync.Modules.EmployeeManagement.Services;

/// <summary>
/// Local file system implementation of IFileStorageService.
/// Stores files in wwwroot/uploads for development purposes.
/// For production, replace with GcpFileStorageService or similar.
/// </summary>
public class LocalFileStorageService : IFileStorageService
{
    private readonly ILogger<LocalFileStorageService> _logger;
    private readonly string _uploadBasePath;
    private readonly string _baseUrl;

    private static readonly Dictionary<string, string> ContentTypeToExtension = new(StringComparer.OrdinalIgnoreCase)
    {
        // Images
        { "image/jpeg", ".jpg" },
        { "image/jpg", ".jpg" },
        { "image/png", ".png" },
        { "image/gif", ".gif" },
        { "image/webp", ".webp" },
        { "image/svg+xml", ".svg" },
        { "image/bmp", ".bmp" },
        
        // Documents
        { "application/pdf", ".pdf" },
        { "application/msword", ".doc" },
        { "application/vnd.openxmlformats-officedocument.wordprocessingml.document", ".docx" },
        { "application/vnd.ms-excel", ".xls" },
        { "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", ".xlsx" },
        { "application/vnd.ms-powerpoint", ".ppt" },
        { "application/vnd.openxmlformats-officedocument.presentationml.presentation", ".pptx" },
        
        // Text
        { "text/plain", ".txt" },
        { "text/csv", ".csv" },
        { "text/html", ".html" },
        { "application/json", ".json" },
        { "application/xml", ".xml" }
    };

    public LocalFileStorageService(
        ILogger<LocalFileStorageService> logger,
        IConfiguration configuration,
        IWebHostEnvironment environment)
    {
        _logger = logger;
        
        // Base path for file storage
        _uploadBasePath = Path.Combine(environment.WebRootPath ?? environment.ContentRootPath, "uploads");
        
        // Base URL for accessing files - configurable for different environments
        _baseUrl = configuration["FileStorage:BaseUrl"] ?? "/uploads";
        
        // Ensure base upload directory exists
        if (!Directory.Exists(_uploadBasePath))
        {
            Directory.CreateDirectory(_uploadBasePath);
            _logger.LogInformation("Created upload base directory: {Path}", _uploadBasePath);
        }
    }

    public async Task<string> SaveFileAsync(
        Stream fileStream, 
        string fileName, 
        string folderName, 
        string? contentType = null)
    {
        try
        {
            // Sanitize folder name and create directory
            var sanitizedFolder = SanitizeFolderName(folderName);
            var folderPath = Path.Combine(_uploadBasePath, sanitizedFolder);
            
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
                _logger.LogInformation("Created folder: {Path}", folderPath);
            }

            // Generate unique file name to prevent conflicts
            var uniqueFileName = GenerateUniqueFileName(fileName, contentType);
            var filePath = Path.Combine(folderPath, uniqueFileName);

            // Save file to disk
            await using var fileOutputStream = new FileStream(filePath, FileMode.Create, FileAccess.Write);
            await fileStream.CopyToAsync(fileOutputStream);

            // Return the accessible URL
            var fileUrl = $"{_baseUrl}/{sanitizedFolder}/{uniqueFileName}";
            
            _logger.LogInformation("File saved successfully: {Url}", fileUrl);
            return fileUrl;
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to save file: {FileName} to folder: {Folder}", fileName, folderName);
            throw;
        }
    }

    public Task<bool> DeleteFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return Task.FromResult(false);
            }

            // Convert URL to file path
            var relativePath = fileUrl.TrimStart('/');
            if (relativePath.StartsWith("uploads/"))
            {
                relativePath = relativePath.Substring("uploads/".Length);
            }
            
            var filePath = Path.Combine(_uploadBasePath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted successfully: {Path}", filePath);
                return Task.FromResult(true);
            }

            _logger.LogWarning("File not found for deletion: {Path}", filePath);
            return Task.FromResult(false);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete file: {Url}", fileUrl);
            return Task.FromResult(false);
        }
    }

    public Task<bool> FileExistsAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(fileUrl))
            {
                return Task.FromResult(false);
            }

            var relativePath = fileUrl.TrimStart('/');
            if (relativePath.StartsWith("uploads/"))
            {
                relativePath = relativePath.Substring("uploads/".Length);
            }
            
            var filePath = Path.Combine(_uploadBasePath, relativePath);
            return Task.FromResult(File.Exists(filePath));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to check file existence: {Url}", fileUrl);
            return Task.FromResult(false);
        }
    }

    public string GetExtensionFromContentType(string contentType)
    {
        if (string.IsNullOrWhiteSpace(contentType))
        {
            return ".bin";
        }

        return ContentTypeToExtension.TryGetValue(contentType, out var extension) 
            ? extension 
            : ".bin";
    }

    private static string SanitizeFolderName(string folderName)
    {
        // Remove invalid path characters and normalize
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(folderName
            .Where(c => !invalidChars.Contains(c))
            .ToArray());
        
        return sanitized.ToLowerInvariant().Replace(" ", "-");
    }

    private string GenerateUniqueFileName(string originalFileName, string? contentType)
    {
        // Get extension from original filename or content type
        var extension = Path.GetExtension(originalFileName);
        if (string.IsNullOrEmpty(extension) && !string.IsNullOrEmpty(contentType))
        {
            extension = GetExtensionFromContentType(contentType);
        }

        // Generate unique name using timestamp and GUID
        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd_HHmmss");
        var uniqueId = Guid.NewGuid().ToString("N")[..8];
        
        // Sanitize original filename for use as prefix
        var sanitizedName = SanitizeFileName(Path.GetFileNameWithoutExtension(originalFileName));
        if (sanitizedName.Length > 50)
        {
            sanitizedName = sanitizedName[..50];
        }

        return $"{sanitizedName}_{timestamp}_{uniqueId}{extension}";
    }

    private static string SanitizeFileName(string fileName)
    {
        var invalidChars = Path.GetInvalidFileNameChars();
        var sanitized = new string(fileName
            .Where(c => !invalidChars.Contains(c))
            .ToArray());
        
        return sanitized.Replace(" ", "_").ToLowerInvariant();
    }
}
