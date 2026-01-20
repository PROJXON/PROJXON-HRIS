using System.Net.Http.Headers;
using System.Text.Json;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Platform.Storage;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using System.Diagnostics;


namespace Client.Services;

/// <summary>
/// Service for handling file picker dialogs and file uploads.
/// Uses Avalonia's StorageProvider for cross-platform file picking.
/// </summary>
public class FileService : IFileService
{
    private readonly ILogger<FileService> _logger;
    private readonly IAuthenticationService _authService;
    private readonly HttpClient _httpClient;
    private readonly string _baseUrl;

    private static readonly FilePickerFileType ImageFileTypes = new("Image Files")
    {
        Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.gif", "*.webp" },
        MimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp" }
    };

    private static readonly FilePickerFileType DocumentFileTypes = new("Document Files")
    {
        Patterns = new[] { "*.pdf", "*.doc", "*.docx" },
        MimeTypes = new[] { "application/pdf", "application/msword", 
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };

    private static readonly FilePickerFileType AllSupportedFileTypes = new("All Supported Files")
    {
        Patterns = new[] { "*.jpg", "*.jpeg", "*.png", "*.gif", "*.webp", "*.pdf", "*.doc", "*.docx" },
        MimeTypes = new[] { "image/jpeg", "image/png", "image/gif", "image/webp",
            "application/pdf", "application/msword",
            "application/vnd.openxmlformats-officedocument.wordprocessingml.document" }
    };

    public FileService(
        ILogger<FileService> logger,
        IAuthenticationService authService,
        IConfiguration configuration,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _authService = authService;
        _httpClient = httpClientFactory.CreateClient("Api");
        _baseUrl = configuration["CloudSyncUrl"] ?? throw new InvalidOperationException("CloudSyncUrl not configured");
    }

    public async Task<IStorageFile?> PickFileAsync(string title, string[]? allowedExtensions = null)
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider == null)
        {
            _logger.LogWarning("Storage provider not available");
            return null;
        }

        var fileTypes = new List<FilePickerFileType>();
        
        if (allowedExtensions != null && allowedExtensions.Length > 0)
        {
            fileTypes.Add(new FilePickerFileType("Allowed Files")
            {
                Patterns = allowedExtensions.Select(e => $"*{e}").ToArray()
            });
        }
        else
        {
            fileTypes.Add(AllSupportedFileTypes);
        }

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = fileTypes
        };

        var result = await storageProvider.OpenFilePickerAsync(options);
        return result.FirstOrDefault();
    }

    public async Task<IReadOnlyList<IStorageFile>> PickFilesAsync(string title, string[]? allowedExtensions = null)
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider == null)
        {
            _logger.LogWarning("Storage provider not available");
            return Array.Empty<IStorageFile>();
        }

        var fileTypes = new List<FilePickerFileType>();
        
        if (allowedExtensions != null && allowedExtensions.Length > 0)
        {
            fileTypes.Add(new FilePickerFileType("Allowed Files")
            {
                Patterns = allowedExtensions.Select(e => $"*{e}").ToArray()
            });
        }
        else
        {
            fileTypes.Add(AllSupportedFileTypes);
        }

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = true,
            FileTypeFilter = fileTypes
        };

        return await storageProvider.OpenFilePickerAsync(options);
    }

    public async Task<IStorageFile?> PickImageAsync(string title = "Select Image")
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider == null)
        {
            _logger.LogWarning("Storage provider not available");
            return null;
        }

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = new[] { ImageFileTypes }
        };

        var result = await storageProvider.OpenFilePickerAsync(options);
        return result.FirstOrDefault();
    }

    public async Task<IStorageFile?> PickDocumentAsync(string title = "Select Document")
    {
        var storageProvider = GetStorageProvider();
        if (storageProvider == null)
        {
            _logger.LogWarning("Storage provider not available");
            return null;
        }

        var options = new FilePickerOpenOptions
        {
            Title = title,
            AllowMultiple = false,
            FileTypeFilter = new[] { DocumentFileTypes }
        };

        var result = await storageProvider.OpenFilePickerAsync(options);
        return result.FirstOrDefault();
    }

    public async Task<FileUploadResult> UploadFileAsync(IStorageFile file, int employeeId, string documentType)
    {
        try
        {
            await using var stream = await file.OpenReadAsync();
            var contentType = GetContentType(file.Name);
            
            return await UploadFileAsync(stream, file.Name, contentType, employeeId, documentType);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", file.Name);
            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }
    
    public void OpenUrl(string url)
    {
        if (string.IsNullOrWhiteSpace(url))
            throw new ArgumentException("URL is empty.", nameof(url));

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
            throw new ArgumentException("Invalid URL.", nameof(url));

        Process.Start(new ProcessStartInfo
        {
            FileName = uri.ToString(),
            UseShellExecute = true
        });
    }


    public async Task<FileUploadResult> UploadFileAsync(
        Stream stream, 
        string fileName, 
        string contentType, 
        int employeeId, 
        string documentType)
    {
        try
        {
            // Get auth token
            var token = await _authService.GetAccessTokenAsync();
            
            // Create multipart form content
            using var content = new MultipartFormDataContent();
            using var streamContent = new StreamContent(stream);
            streamContent.Headers.ContentType = new MediaTypeHeaderValue(contentType);
            
            content.Add(streamContent, "file", fileName);

            // Configure request
            using var request = new HttpRequestMessage(
                HttpMethod.Post, 
                $"{_baseUrl}api/Document/upload/{employeeId}/{documentType}");
            
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            request.Content = content;

            // Send request
            var response = await _httpClient.SendAsync(request);
            var responseBody = await response.Content.ReadAsStringAsync();

            if (response.IsSuccessStatusCode)
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var uploadResponse = JsonSerializer.Deserialize<DocumentUploadApiResponse>(responseBody, options);
                
                _logger.LogInformation(
                    "File uploaded successfully: {FileName} -> {Url}",
                    fileName, uploadResponse?.FileUrl);

                return new FileUploadResult
                {
                    Success = true,
                    FileUrl = uploadResponse?.FileUrl,
                    FileName = uploadResponse?.FileName ?? fileName,
                    FileSizeBytes = uploadResponse?.FileSizeBytes ?? 0
                };
            }
            else
            {
                _logger.LogWarning(
                    "File upload failed: {Status} - {Body}",
                    response.StatusCode, responseBody);

                return new FileUploadResult
                {
                    Success = false,
                    ErrorMessage = $"Upload failed: {response.StatusCode}"
                };
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to upload file: {FileName}", fileName);
            return new FileUploadResult
            {
                Success = false,
                ErrorMessage = ex.Message
            };
        }
    }

    private IStorageProvider? GetStorageProvider()
    {
        if (Application.Current?.ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            return desktop.MainWindow?.StorageProvider;
        }
        
        // For single view applications (mobile)
        if (Application.Current?.ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            return (singleView.MainView as TopLevel)?.StorageProvider;
        }

        return null;
    }

    private static string GetContentType(string fileName)
    {
        var extension = Path.GetExtension(fileName).ToLowerInvariant();
        return extension switch
        {
            ".jpg" or ".jpeg" => "image/jpeg",
            ".png" => "image/png",
            ".gif" => "image/gif",
            ".webp" => "image/webp",
            ".pdf" => "application/pdf",
            ".doc" => "application/msword",
            ".docx" => "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
            _ => "application/octet-stream"
        };
    }

    private class DocumentUploadApiResponse
    {
        public bool Success { get; set; }
        public string? FileUrl { get; set; }
        public string? FileName { get; set; }
        public string? DocumentType { get; set; }
        public string? ContentType { get; set; }
        public long FileSizeBytes { get; set; }
    }
}
