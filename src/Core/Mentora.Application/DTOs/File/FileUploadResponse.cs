namespace Mentora.Application.DTOs.File;

public class FileUploadResponse
{
    public string FileUrl { get; set; } = null!;
    public string FileName { get; set; } = null!;
    public long FileSize { get; set; }
    public string ContentType { get; set; } = null!;
}