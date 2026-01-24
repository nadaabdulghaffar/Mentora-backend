using FluentValidation;
using Mentora.Application.DTOs.Auth;
using FluentValidation;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.File;
using Mentora.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace Mentora.Application.Validators;
public class FileUploadRequestValidator : AbstractValidator<FileUploadRequest>
{
    private readonly string[] _allowedCvExtensions = { ".pdf", ".doc", ".docx" };
    private readonly string[] _allowedImageExtensions = { ".jpg", ".jpeg", ".png" };
    private const long MaxCvFileSize = 5 * 1024 * 1024; // 5 MB
    private const long MaxImageFileSize = 2 * 1024 * 1024; // 2 MB

    public FileUploadRequestValidator()
    {
        RuleFor(x => x.File)
            .NotNull()
            .WithMessage("File is required");

        RuleFor(x => x.FileType)
            .NotEmpty()
            .WithMessage("File type is required")
            .Must(x => x == "cv" || x == "profile-picture")
            .WithMessage("File type must be 'cv' or 'profile-picture'");

        RuleFor(x => x.File)
            .Must((request, file) => BeValidFileExtension(file, request.FileType))
            .When(x => x.File != null)
            .WithMessage("Invalid file extension");

        RuleFor(x => x.File)
            .Must((request, file) => BeValidFileSize(file, request.FileType))
            .When(x => x.File != null)
            .WithMessage(x => x.FileType == "cv"
                ? "CV file size must not exceed 5 MB"
                : "Image file size must not exceed 2 MB");
    }

    private bool BeValidFileExtension(IFormFile file, string fileType)
    {
        var extension = Path.GetExtension(file.FileName).ToLower();

        if (fileType == "cv")
            return _allowedCvExtensions.Contains(extension);

        if (fileType == "profile-picture")
            return _allowedImageExtensions.Contains(extension);

        return false;
    }

    private bool BeValidFileSize(IFormFile file, string fileType)
    {
        if (fileType == "cv")
            return file.Length <= MaxCvFileSize;

        if (fileType == "profile-picture")
            return file.Length <= MaxImageFileSize;

        return false;
    }
}