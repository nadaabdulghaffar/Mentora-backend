namespace Mentora.Infrastructure.Services;

using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Mentora.Application.DTOs.Common;
using Mentora.Application.DTOs.File;
using Mentora.Application.Interfaces;


public class FileUploadService : IFileUploadService
{
    private readonly IWebHostEnvironment _environment;
    private readonly ILogger<FileUploadService> _logger;
    private readonly string _uploadPath;

    private readonly string[] _allowedCvExtensions = { ".pdf", ".doc", ".docx" };
    private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxCvFileSize = 5 * 1024 * 1024; // 5 MB
    private const long MaxImageFileSize = 2 * 1024 * 1024; // 2 MB

    public FileUploadService(IWebHostEnvironment environment, ILogger<FileUploadService> logger)
    {
        _environment = environment;
        _logger = logger;

        var webRoot = _environment.WebRootPath ?? Path.Combine(_environment.ContentRootPath, "wwwroot");
        _uploadPath = Path.Combine(webRoot, "uploads");

        // Ensure upload directories exist
        Directory.CreateDirectory(Path.Combine(_uploadPath, "cvs"));
        Directory.CreateDirectory(Path.Combine(_uploadPath, "profile-pictures"));
    }

    public async Task<ApiResponse<FileUploadResponse>> UploadFileAsync(
        IFormFile file,
        string fileType,
        Guid userId)
    {
        try
        {
            // Validate file
            var validationResult = await ValidateFileAsync(file, fileType);
            if (!validationResult.Success)
            {
                return ApiResponse<FileUploadResponse>.ErrorResponse(validationResult.Message);
            }

            // Determine folder
            var folder = fileType == "cv" ? "cvs" : "profile-pictures";
            var folderPath = Path.Combine(_uploadPath, folder);

            // Generate unique filename
            var extension = Path.GetExtension(file.FileName);
            var fileName = $"{userId}_{DateTime.UtcNow:yyyyMMddHHmmss}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(folderPath, fileName);

            // Save file
            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            // Generate URL
            var fileUrl = $"/uploads/{folder}/{fileName}";

            _logger.LogInformation(
                "File uploaded successfully. User: {UserId}, Type: {FileType}, URL: {FileUrl}",
                userId, fileType, fileUrl
            );

            var response = new FileUploadResponse
            {
                FileUrl = fileUrl,
                FileName = file.FileName,
                FileSize = file.Length,
                ContentType = file.ContentType
            };

            return ApiResponse<FileUploadResponse>.SuccessResponse(response, "File uploaded successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error uploading file for user {UserId}", userId);
            return ApiResponse<FileUploadResponse>.ErrorResponse($"File upload failed: {ex.Message}");
        }
    }

    public async Task<ApiResponse<bool>> DeleteFileAsync(string fileUrl)
    {
        try
        {
            if (string.IsNullOrEmpty(fileUrl))
                return ApiResponse<bool>.SuccessResponse(true, "No file to delete");

            // Convert URL to physical path
            var relativePath = fileUrl.TrimStart('/').Replace("/", Path.DirectorySeparatorChar.ToString());
            var filePath = Path.Combine(_environment.WebRootPath, relativePath);

            if (File.Exists(filePath))
            {
                File.Delete(filePath);
                _logger.LogInformation("File deleted: {FilePath}", filePath);
            }

            return ApiResponse<bool>.SuccessResponse(true, "File deleted successfully");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error deleting file {FileUrl}", fileUrl);
            return ApiResponse<bool>.ErrorResponse($"File deletion failed: {ex.Message}");
        }
    }

    public Task<ApiResponse<bool>> ValidateFileAsync(IFormFile file, string fileType)
    {
        if (file == null || file.Length == 0)
        {
            return Task.FromResult(ApiResponse<bool>.ErrorResponse("File is required"));
        }

        // Validate file extension
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (fileType == "cv")
        {
            if (!_allowedCvExtensions.Contains(extension))
            {
                return Task.FromResult(ApiResponse<bool>.ErrorResponse(
                    $"Invalid file type. Allowed: {string.Join(", ", _allowedCvExtensions)}"
                ));
            }

            if (file.Length > MaxCvFileSize)
            {
                return Task.FromResult(ApiResponse<bool>.ErrorResponse(
                    "CV file size must not exceed 5 MB"
                ));
            }
        }
        else if (fileType == "profile-picture")
        {
            if (!_allowedImageExtensions.Contains(extension))
            {
                return Task.FromResult(ApiResponse<bool>.ErrorResponse(
                    $"Invalid file type. Allowed: {string.Join(", ", _allowedImageExtensions)}"
                ));
            }

            if (file.Length > MaxImageFileSize)
            {
                return Task.FromResult(ApiResponse<bool>.ErrorResponse(
                    "Image file size must not exceed 2 MB"
                ));
            }
        }
        else
        {
            return Task.FromResult(ApiResponse<bool>.ErrorResponse(
                "Invalid file type. Must be 'cv' or 'profile-picture'"
            ));
        }

        return Task.FromResult(ApiResponse<bool>.SuccessResponse(true, "File is valid"));
    }
}