using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.Interfaces;
using System.Security.Claims;

namespace Mentora.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly IAuthService _authService;

    public AuthController(IAuthService authService)
    {
        _authService = authService;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterInitial([FromBody] RegisterInitialRequest request)
    {
        var result = await _authService.RegisterInitialAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("verify-email")]
    public async Task<IActionResult> VerifyEmail([FromBody] VerifyEmailRequest request)
    {
        var result = await _authService.VerifyEmailAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("resend-verification")]
    public async Task<IActionResult> ResendVerification([FromBody] string email)
    {
        var result = await _authService.ResendVerificationEmailAsync(email);
        return Ok(result);
    }

    [HttpPost("complete-registration")]
    public async Task<IActionResult> CompleteRegistration([FromBody] CompleteRegistrationRequest request)
    {
        var result = await _authService.CompleteRegistrationAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("complete-mentee-profile")]
    public async Task<IActionResult> CompleteMenteeProfile([FromBody] CompleteMenteeProfileRequest request)
    {
        var result = await _authService.CompleteMenteeProfileAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("complete-mentor-profile")]
    public async Task<IActionResult> CompleteMentorProfile([FromBody] CompleteMentorProfileRequest request)
    {
        var result = await _authService.CompleteMentorProfileAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
}