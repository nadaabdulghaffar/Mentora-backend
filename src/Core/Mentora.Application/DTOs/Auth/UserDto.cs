namespace Mentora.Application.DTOs.Auth;

public class UserDto
{
    public Guid UserId { get; set; }
    public string Email { get; set; } = null!;
    public string FirstName { get; set; } = null!;
    public string LastName { get; set; } = null!;
    public string? Role { get; set; } // Nullable for users who haven't selected role yet
    public bool IsEmailVerified { get; set; }
    public bool IsActive { get; set; }
    public DateTime CreatedAt { get; set; }

    public string? RegistrationStep { get; set; }   // Current registration step for incomplete registrations
    public string? NextStep { get; set; } // Next action required in registration flow
}
