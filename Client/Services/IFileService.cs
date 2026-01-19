using Avalonia.Platform.Storage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
namespace Client.Services;

/// <summary>
/// Interface for file handling operations in the client application.
/// Wraps platform-specific file picker and upload functionality.
/// </summary>
public interface IFileService
{
    /// <summary>
    /// Opens a file picker dialog to select a single file.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="allowedExtensions">Allowed file extensions (e.g., ".jpg", ".png").</param>
    /// <returns>The selected file info, or null if cancelled.</returns>
    Task<IStorageFile?> PickFileAsync(string title, string[]? allowedExtensions = null);
    
    /// <summary>
    /// Opens a file picker dialog to select multiple files.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <param name="allowedExtensions">Allowed file extensions.</param>
    /// <returns>List of selected files.</returns>
    Task<IReadOnlyList<IStorageFile>> PickFilesAsync(string title, string[]? allowedExtensions = null);
    
    /// <summary>
    /// Opens a file picker specifically for images.
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <returns>The selected image file, or null if cancelled.</returns>
    Task<IStorageFile?> PickImageAsync(string title = "Select Image");
    
    /// <summary>
    /// Opens a file picker specifically for documents (PDF, Word, etc.).
    /// </summary>
    /// <param name="title">Dialog title.</param>
    /// <returns>The selected document file, or null if cancelled.</returns>
    Task<IStorageFile?> PickDocumentAsync(string title = "Select Document");
    
    /// <summary>
    /// Uploads a file to the backend API.
    /// </summary>
    /// <param name="file">The file to upload.</param>
    /// <param name="employeeId">The employee ID to associate the file with.</param>
    /// <param name="documentType">The type of document (profile-picture, resume, etc.).</param>
    /// <returns>The upload result containing the file URL.</returns>
    Task<FileUploadResult> UploadFileAsync(IStorageFile file, int employeeId, string documentType);
    
    /// <summary>
    /// Uploads a file stream to the backend API.
    /// </summary>
    /// <param name="stream">The file stream.</param>
    /// <param name="fileName">Original file name.</param>
    /// <param name="contentType">MIME type of the file.</param>
    /// <param name="employeeId">The employee ID.</param>
    /// <param name="documentType">The type of document.</param>
    /// <returns>The upload result.</returns>
    Task<FileUploadResult> UploadFileAsync(Stream stream, string fileName, string contentType, int employeeId, string documentType);
    
    /// <summary>
    /// Opens a URL (typically a document link) in the user's default browser.
    /// </summary>
    /// <param name="url">The URL to open.</param>
    void OpenUrl(string url);
}

/// <summary>
/// Result of a file upload operation.
/// </summary>
public class FileUploadResult
{
    public bool Success { get; set; }
    public string? FileUrl { get; set; }
    public string? FileName { get; set; }
    public string? ErrorMessage { get; set; }
    public long FileSizeBytes { get; set; }
}
