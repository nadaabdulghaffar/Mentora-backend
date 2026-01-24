namespace Mentora.Application.Interfaces;

using Microsoft.AspNetCore.Http;
using Mentora.Application.DTOs.Common;
using Mentora.Application.DTOs.File;

public interface IFileUploadService
{
    Task<ApiResponse<FileUploadResponse>> UploadFileAsync(IFormFile file, string fileType, Guid userId);
    Task<ApiResponse<bool>> DeleteFileAsync(string fileUrl);
    Task<ApiResponse<bool>> ValidateFileAsync(IFormFile file, string fileType);
}
