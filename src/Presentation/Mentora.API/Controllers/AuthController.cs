using Mentora.API.Extensions;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.Interfaces;
using Mentora.Domain.Entities;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

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

    public async Task<IActionResult> ResendVerification([FromBody] ResendVerificationRequest request)
    {
        var result = await _authService.ResendVerificationEmailAsync(request);
        return Ok(result);
    }

    [HttpPost("select-role")]
    public async Task<IActionResult> SelectRole([FromBody] SelectRoleRequest request)
    {
        var result = await _authService.SelectRoleAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("complete-mentee-profile")]
    public async Task<IActionResult> CompleteMenteeProfile([FromBody] CompleteMenteeProfileRequest request)
    {
        var result = await _authService.CompleteMenteeProfileProgressiveAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpPost("complete-mentor-profile")]
    public async Task<IActionResult> CompleteMentorProfile([FromBody] CompleteMentorProfileRequest request)
    {
        var result = await _authService.CompleteMentorProfileProgressiveAsync(request);

        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }

    [HttpGet("login-google")]
    public IActionResult LoginGoogle(bool rememberMe = false)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GoogleResponse")
        };

        properties.Items.Add("rememberMe", rememberMe.ToString());

        return Challenge(properties, GoogleDefaults.AuthenticationScheme);
    }

    [HttpGet("google-response")]
    public async Task<IActionResult> GoogleResponse()
    {
   
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)
            return BadRequest("Google Authentication Failed");

        bool rememberMe = false;
        if (result.Properties.Items.ContainsKey("rememberMe"))
        {
            bool.TryParse(result.Properties.Items["rememberMe"], out rememberMe);
        }

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var firstName = result.Principal.FindFirstValue(ClaimTypes.GivenName) ?? "User";
        var lastName = result.Principal.FindFirstValue(ClaimTypes.Surname) ?? "";

        var authResult = await _authService.ExternalLoginAsync(email, firstName, lastName, "Google", rememberMe);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authResult.Success)
            return BadRequest(authResult.Message);

        var redirectUrl = $"https://localhost:3000/auth-success?token={authResult.Data.AccessToken}&refreshToken={authResult.Data.RefreshToken}";
        return Redirect(redirectUrl);
    }

    [HttpGet("login-github")]
    public IActionResult LoginGitHub(bool rememberMe = false)
    {
        var properties = new AuthenticationProperties
        {
            RedirectUri = Url.Action("GitHubResponse") 
        };

        properties.Items.Add("rememberMe", rememberMe.ToString());

        return Challenge(properties, "GitHub");
    }

    [HttpGet("github-response")]
    public async Task<IActionResult> GitHubResponse()
    {
      
        var result = await HttpContext.AuthenticateAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!result.Succeeded)

            return BadRequest("GitHub Authentication Failed");

       
        bool rememberMe = false;
        if (result.Properties.Items.ContainsKey("rememberMe"))
        {
            bool.TryParse(result.Properties.Items["rememberMe"], out rememberMe);
        }

        var email = result.Principal.FindFirstValue(ClaimTypes.Email);
        var name = result.Principal.FindFirstValue(ClaimTypes.Name) ?? "GitHub User";

        var authResult = await _authService.ExternalLoginAsync(email, name, "", "GitHub", rememberMe);

        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);

        if (!authResult.Success) return BadRequest(authResult.Message);

        var redirectUrl = $"https://localhost:3000/auth-success?token={authResult.Data.AccessToken}&refreshToken={authResult.Data.RefreshToken}";
        return Redirect(redirectUrl);
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login(LoginRequest request)
    {
        var result = await _authService.LoginAsync(request);
        if (!result.Success)
            return BadRequest(result);

        return Ok(result);
    }
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody]RefreshTokenRequest request)
    {
     

        var result = await _authService.RefreshTokenAsync(request);

        if (!result.Success)
        {
            return Unauthorized(result);
        }

        return Ok(result);
    }
    [Authorize]
    [HttpPost("logout")]
  
    public async Task<IActionResult> Logout([FromBody] RefreshTokenRequest request)
    {
        await _authService.LogoutAsync(request);
        return Ok(new { message = "Logged out successfully" });

    }
    [HttpPost("forgot-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
    {
        var result = await _authService.ForgotPasswordAsync(request.Email);

        return Ok(result);
    }


    [HttpPost("reset-password")]
    [AllowAnonymous]
    public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
    {
        var result = await _authService.ResetPasswordAsync(request);

        if (!result.Success)
        {
            return BadRequest(result);
        }

        return Ok(result);

    }
    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser()
    {
        var userIdString = User.FindFirstValue(ClaimTypes.NameIdentifier);

        if (string.IsNullOrEmpty(userIdString) || !Guid.TryParse(userIdString, out var userId))
        {
            return Unauthorized(new { message = "Invalid token: User ID is missing." });
        }

        var result = await _authService.GetCurrentUserAsync(userId);

        if (!result.Success)
        {
            return NotFound(result);
        }

        return Ok(result);
    }
}