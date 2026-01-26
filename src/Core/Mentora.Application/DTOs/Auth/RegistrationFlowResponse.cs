namespace Mentora.Application.DTOs.Auth;

public class RegistrationFlowResponse
{
    public string RegistrationToken { get; set; } = null!;  // Temporary token for this registration session
    public string CurrentStep { get; set; } = null!;        // EmailVerified, RoleSelected, etc.
    public string NextStep { get; set; } = null!;           // What to do next
    public DateTime ExpiresAt { get; set; }                 // When the registration token expires
    public UserBasicInfo User { get; set; } = null!;
}

public class UserBasicInfo
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Role { get; set; }  // Null until role is selected
}
