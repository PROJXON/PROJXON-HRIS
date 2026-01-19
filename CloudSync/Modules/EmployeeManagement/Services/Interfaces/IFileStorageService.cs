namespace CloudSync.Modules.EmployeeManagement.Services.Interfaces;

/// <summary>
/// File storage service interface implementing the Strategy Pattern.
/// This allows for easy migration from local storage to cloud providers (GCP, AWS, Azure).
/// </summary>
public interface IFileStorageService
{
    /// <summary>
    /// Saves a file to storage and returns the accessible URL.
    /// </summary>
    /// <param name="fileStream">The file content stream.</param>
    /// <param name="fileName">Original file name.</param>
    /// <param name="folderName">Folder/container name for organization (e.g., "profile-pictures", "documents").</param>
    /// <param name="contentType">MIME type of the file.</param>
    /// <returns>The publicly accessible URL of the stored file.</returns>
    Task<string> SaveFileAsync(Stream fileStream, string fileName, string folderName, string? contentType = null);

    /// <summary>
    /// Deletes a file from storage.
    /// </summary>
    /// <param name="fileUrl">The URL or path of the file to delete.</param>
    /// <returns>True if deletion was successful, false otherwise.</returns>
    Task<bool> DeleteFileAsync(string fileUrl);

    /// <summary>
    /// Checks if a file exists in storage.
    /// </summary>
    /// <param name="fileUrl">The URL or path of the file.</param>
    /// <returns>True if the file exists, false otherwise.</returns>
    Task<bool> FileExistsAsync(string fileUrl);

    /// <summary>
    /// Gets the file extension from a content type.
    /// </summary>
    /// <param name="contentType">The MIME type.</param>
    /// <returns>The file extension including the dot (e.g., ".pdf").</returns>
    string GetExtensionFromContentType(string contentType);
}
