namespace Mentora.API.Controllers;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mentora.API.Extensions;
using Mentora.Application.Interfaces;


[ApiController]
[Route("api/[controller]")]
[Authorize]
public class FileController : ControllerBase
{
    private readonly IFileUploadService _fileUploadService;
    private readonly IUnitOfWork _unitOfWork;
    private readonly ILogger<FileController> _logger;

    public FileController(IFileUploadService fileUploadService, IUnitOfWork unitOfWork, ILogger<FileController> logger)
    {
        _fileUploadService = fileUploadService;
        _unitOfWork = unitOfWork;
        _logger = logger;
    }

    [HttpPost("upload-cv")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadCv(IFormFile file, [FromQuery] string? registrationToken)
    {
        _logger.LogInformation("CV Upload attempt. File: {FileName}, ContentType: {ContentType}", file?.FileName, file?.ContentType);

        // 1. Try to get token from Query String (handled by binding)
        if (!string.IsNullOrEmpty(registrationToken))
        {
             _logger.LogInformation("Token found in Query: {Token}", registrationToken);
        }

        // 2. Try to get token from header
        if (string.IsNullOrEmpty(registrationToken))
        {
            registrationToken = Request.Headers["X-Registration-Token"].FirstOrDefault() 
                               ?? Request.Headers["Registration-Token"].FirstOrDefault();
            if (!string.IsNullOrEmpty(registrationToken))
                _logger.LogInformation("Token found in Header");
        }
        
        // 3. Try to get token from Form Data (multi-part body)
        if (string.IsNullOrEmpty(registrationToken) && Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            registrationToken = form["registrationToken"].FirstOrDefault() 
                               ?? form["RegistrationToken"].FirstOrDefault()
                               ?? form["token"].FirstOrDefault();
            
            if (!string.IsNullOrEmpty(registrationToken))
            {
                _logger.LogInformation("Token found in Form");
            }
            else
            {
                _logger.LogInformation("Token not found in Form. Available keys: {Keys}", string.Join(", ", form.Keys));
            }
        }

        var (userId, error) = await ResolveUserIdExtAsync(registrationToken);

        if (userId == Guid.Empty)
        {
            _logger.LogWarning("Unauthorized CV upload attempt: {Error}", error);
            return Unauthorized(new { message = error });
        }

        var result = await _fileUploadService.UploadFileAsync(file, "cv", userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("upload-profile-picture")]
    [AllowAnonymous]
    public async Task<IActionResult> UploadProfilePicture(IFormFile file, [FromQuery] string? registrationToken)
    {
        // Use same logic as UploadCv for consistency
        if (string.IsNullOrEmpty(registrationToken))
        {
            registrationToken = Request.Headers["X-Registration-Token"].FirstOrDefault()
                               ?? Request.Headers["Registration-Token"].FirstOrDefault();
        }

        if (string.IsNullOrEmpty(registrationToken) && Request.HasFormContentType)
        {
            var form = await Request.ReadFormAsync();
            registrationToken = form["registrationToken"].FirstOrDefault() 
                               ?? form["RegistrationToken"].FirstOrDefault();
        }

        var (userId, error) = await ResolveUserIdExtAsync(registrationToken);

        if (userId == Guid.Empty)
        {
            return Unauthorized(new { message = error });
        }

        var result = await _fileUploadService.UploadFileAsync(file, "profile-picture", userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteFile([FromQuery] string fileUrl)
    {
        var result = await _fileUploadService.DeleteFileAsync(fileUrl);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    private async Task<(Guid UserId, string? Error)> ResolveUserIdExtAsync(string? registrationToken)
    {
        // 1. Try to get user ID from JWT token (if provided)
        var userId = User.GetUserId();
        if (userId != Guid.Empty)
        {
            _logger.LogInformation("UserId resolved from JWT: {UserId}", userId);
            return (userId, null);
        }

        // 2. Try to get user ID from registration token
        if (string.IsNullOrEmpty(registrationToken))
        {
            return (Guid.Empty, "No authentication provided. Please login or provide a registration token.");
        }

        // Normalize token (handle Base64 spaces from query string)
        var normalizedToken = registrationToken.Trim().Replace(" ", "+");

        var session = await _unitOfWork.RegistrationSessions.GetByTokenAsync(normalizedToken);
        
        if (session == null)
        {
            _logger.LogWarning("No active session found for token: {Token}", normalizedToken);
            return (Guid.Empty, "Invalid registration token.");
        }

        if (session.ExpiresAt <= DateTime.UtcNow)
        {
            return (Guid.Empty, "Registration token has expired.");
        }

        if (session.IsCompleted)
        {
            return (Guid.Empty, "Registration is already complete. Please log in to upload files.");
        }

        return (session.UserId, null);
    }
}
