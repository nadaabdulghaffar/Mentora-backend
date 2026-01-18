namespace Mentora.Application.DTOs.Auth;

public class CompleteRegistrationRequest
{
    public Guid UserId { get; set; }
    public string Role { get; set; } = null!; // "mentee" or "mentor"
}
