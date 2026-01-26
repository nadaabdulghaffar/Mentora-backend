namespace Mentora.Application.DTOs.Auth;

public class SelectRoleRequest
{
    // No longer needs userId - extracted from registration token
    public string RegistrationToken { get; set; } = null!;  // From previous step
    public string Role { get; set; } = null!;  // "mentee" or "mentor"
}
