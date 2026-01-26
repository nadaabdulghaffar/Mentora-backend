namespace Mentora.Application.DTOs.Auth;

public class RegistrationCompleteResponse
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string Role { get; set; } = null!;

    // Full authentication tokens (no longer temporary)
    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public DateTime ExpiresAt { get; set; }

    public string Message { get; set; } = "Registration completed successfully! Welcome aboard.";
}