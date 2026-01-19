using CloudSync.Modules.EmployeeManagement.Repositories.Interfaces;
using CloudSync.Modules.EmployeeManagement.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CloudSync.Modules.EmployeeManagement.Controllers;

/// <summary>
/// Controller for managing employee documents and file uploads.
/// </summary>
[ApiController]
[Route("api/[controller]")]
[Authorize]
public class DocumentController : ControllerBase
{
    private readonly IFileStorageService _fileStorageService;
    private readonly IEmployeeRepository _employeeRepository;
    private readonly ILogger<DocumentController> _logger;

    // Allowed document types and their corresponding folder names
    private static readonly Dictionary<string, string> DocumentTypeFolders = new(StringComparer.OrdinalIgnoreCase)
    {
        { "profile-picture", "profile-pictures" },
        { "resume", "resumes" },
        { "cover-letter", "cover-letters" },
        { "contract", "contracts" },
        { "certification", "certifications" },
        { "id-document", "id-documents" },
        { "other", "other-documents" }
    };

    // Maximum file size (10 MB)
    private const long MaxFileSize = 10 * 1024 * 1024;

    // Allowed content types
    private static readonly HashSet<string> AllowedImageTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "image/jpeg", "image/jpg", "image/png", "image/gif", "image/webp"
    };

    private static readonly HashSet<string> AllowedDocumentTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        "application/pdf",
        "application/msword",
        "application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        "image/jpeg", "image/jpg", "image/png", "image/gif"
    };

    public DocumentController(
        IFileStorageService fileStorageService,
        IEmployeeRepository employeeRepository,
        ILogger<DocumentController> logger)
    {
        _fileStorageService = fileStorageService;
        _employeeRepository = employeeRepository;
        _logger = logger;
    }

    /// <summary>
    /// Uploads a document for an employee.
    /// </summary>
    /// <param name="employeeId">The employee's ID.</param>
    /// <param name="documentType">Type of document: profile-picture, resume, cover-letter, contract, certification, id-document, other.</param>
    /// <param name="file">The file to upload.</param>
    /// <returns>The URL of the uploaded file.</returns>
    [HttpPost("upload/{employeeId:int}/{documentType}")]
    [RequestSizeLimit(MaxFileSize)]
    public async Task<IActionResult> UploadDocument(
        int employeeId, 
        string documentType, 
        IFormFile file)
    {
        try
        {
            // Validate document type
            if (!DocumentTypeFolders.TryGetValue(documentType, out var folderName))
            {
                return BadRequest(new { 
                    Error = "Invalid document type", 
                    AllowedTypes = DocumentTypeFolders.Keys 
                });
            }

            // Validate file
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { Error = "No file provided or file is empty" });
            }

            if (file.Length > MaxFileSize)
            {
                return BadRequest(new { 
                    Error = "File too large", 
                    MaxSizeBytes = MaxFileSize,
                    MaxSizeMB = MaxFileSize / (1024 * 1024)
                });
            }

            // Validate content type based on document type
            var isValidContentType = documentType.Equals("profile-picture", StringComparison.OrdinalIgnoreCase)
                ? AllowedImageTypes.Contains(file.ContentType)
                : AllowedDocumentTypes.Contains(file.ContentType);

            if (!isValidContentType)
            {
                var allowedTypes = documentType.Equals("profile-picture", StringComparison.OrdinalIgnoreCase)
                    ? AllowedImageTypes
                    : AllowedDocumentTypes;
                    
                return BadRequest(new { 
                    Error = "Invalid file type", 
                    ContentType = file.ContentType,
                    AllowedTypes = allowedTypes 
                });
            }

            // Verify employee exists
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { Error = "Employee not found", EmployeeId = employeeId });
            }

            // Delete old file if exists for this document type
            var oldFileUrl = GetCurrentDocumentUrl(employee, documentType);
            if (!string.IsNullOrEmpty(oldFileUrl))
            {
                await _fileStorageService.DeleteFileAsync(oldFileUrl);
                _logger.LogInformation("Deleted old file: {Url}", oldFileUrl);
            }

            // Save new file
            await using var stream = file.OpenReadStream();
            var fileUrl = await _fileStorageService.SaveFileAsync(
                stream, 
                file.FileName, 
                $"employees/{employeeId}/{folderName}",
                file.ContentType);

            // Update employee record with new URL
            await UpdateEmployeeDocumentUrl(employee, documentType, fileUrl);

            _logger.LogInformation(
                "Document uploaded successfully. EmployeeId: {EmployeeId}, Type: {Type}, Url: {Url}",
                employeeId, documentType, fileUrl);

            return Ok(new DocumentUploadResponse
            {
                Success = true,
                FileUrl = fileUrl,
                FileName = file.FileName,
                DocumentType = documentType,
                ContentType = file.ContentType,
                FileSizeBytes = file.Length
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, 
                "Failed to upload document. EmployeeId: {EmployeeId}, Type: {Type}", 
                employeeId, documentType);
            
            return StatusCode(500, new { 
                Error = "Failed to upload document", 
                Details = ex.Message 
            });
        }
    }

    /// <summary>
    /// Deletes a document for an employee.
    /// </summary>
    [HttpDelete("{employeeId:int}/{documentType}")]
    public async Task<IActionResult> DeleteDocument(int employeeId, string documentType)
    {
        try
        {
            if (!DocumentTypeFolders.ContainsKey(documentType))
            {
                return BadRequest(new { Error = "Invalid document type" });
            }

            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { Error = "Employee not found" });
            }

            var fileUrl = GetCurrentDocumentUrl(employee, documentType);
            if (string.IsNullOrEmpty(fileUrl))
            {
                return NotFound(new { Error = "Document not found for this employee" });
            }

            var deleted = await _fileStorageService.DeleteFileAsync(fileUrl);
            if (deleted)
            {
                await UpdateEmployeeDocumentUrl(employee, documentType, null);
                return Ok(new { Success = true, Message = "Document deleted successfully" });
            }

            return StatusCode(500, new { Error = "Failed to delete document" });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to delete document for employee {EmployeeId}", employeeId);
            return StatusCode(500, new { Error = "Failed to delete document" });
        }
    }

    /// <summary>
    /// Gets all document URLs for an employee.
    /// </summary>
    [HttpGet("{employeeId:int}")]
    public async Task<IActionResult> GetEmployeeDocuments(int employeeId)
    {
        try
        {
            var employee = await _employeeRepository.GetByIdAsync(employeeId);
            if (employee == null)
            {
                return NotFound(new { Error = "Employee not found" });
            }

            var documents = new Dictionary<string, string?>();
            foreach (var docType in DocumentTypeFolders.Keys)
            {
                documents[docType] = GetCurrentDocumentUrl(employee, docType);
            }

            return Ok(new { EmployeeId = employeeId, Documents = documents });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get documents for employee {EmployeeId}", employeeId);
            return StatusCode(500, new { Error = "Failed to retrieve documents" });
        }
    }

    private string? GetCurrentDocumentUrl(Models.Employee employee, string documentType)
    {
        if (employee.Documents == null)
        {
            return null;
        }

        return documentType.ToLowerInvariant() switch
        {
            "profile-picture" => employee.Documents.ProfilePictureUrl,
            "resume" => employee.Documents.ResumeUrl,
            "cover-letter" => employee.Documents.CoverLetterUrl,
            "linkedin" => employee.Documents.LinkedInUrl,
            "github" => employee.Documents.GitHubUrl,
            "personal-website" => employee.Documents.PersonalWebsiteUrl,
            _ => null
        };
    }

    private async Task UpdateEmployeeDocumentUrl(Models.Employee employee, string documentType, string? url)
    {
        // Initialize Documents if null
        employee.Documents ??= new Models.EmployeeDocuments();

        switch (documentType.ToLowerInvariant())
        {
            case "profile-picture":
                employee.Documents.ProfilePictureUrl = url;
                break;
            case "resume":
                employee.Documents.ResumeUrl = url;
                break;
            case "cover-letter":
                employee.Documents.CoverLetterUrl = url;
                break;
            case "linkedin":
                employee.Documents.LinkedInUrl = url;
                break;
            case "github":
                employee.Documents.GitHubUrl = url;
                break;
            case "personal-website":
                employee.Documents.PersonalWebsiteUrl = url;
                break;
        }

        await _employeeRepository.UpdateAsync(employee.Id, employee);
    }
}

/// <summary>
/// Response model for document upload operations.
/// </summary>
public class DocumentUploadResponse
{
    public bool Success { get; set; }
    public string FileUrl { get; set; } = string.Empty;
    public string FileName { get; set; } = string.Empty;
    public string DocumentType { get; set; } = string.Empty;
    public string ContentType { get; set; } = string.Empty;
    public long FileSizeBytes { get; set; }
}
