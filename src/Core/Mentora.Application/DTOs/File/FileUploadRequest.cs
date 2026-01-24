using Microsoft.AspNetCore.Http;

namespace Mentora.Application.DTOs.File;

public class FileUploadRequest
{
    public IFormFile File { get; set; } = null!;
    public string FileType { get; set; } = null!;  // "cv", "profile-picture"
}