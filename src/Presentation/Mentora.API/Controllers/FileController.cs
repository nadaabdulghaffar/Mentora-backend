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

    public FileController(IFileUploadService fileUploadService)
    {
        _fileUploadService = fileUploadService;
    }

    [HttpPost("upload-cv")]
    public async Task<IActionResult> UploadCv([FromForm] IFormFile file)
    {
        var userId = User.GetUserId();

        var result = await _fileUploadService.UploadFileAsync(file, "cv", userId);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("upload-profile-picture")]
    public async Task<IActionResult> UploadProfilePicture([FromForm] IFormFile file)
    {
        var userId = User.GetUserId();

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
}