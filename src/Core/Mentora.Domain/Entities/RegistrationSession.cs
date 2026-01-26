namespace Mentora.Domain.Entities;
using Mentora.Domain.Enums;

public class RegistrationSession
{
    public Guid SessionId { get; set; }
    public Guid UserId { get; set; }
    public string SessionToken { get; set; } = null!;  // Temporary token for registration flow
    public RegistrationStep CurrentStep { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }  // Session expires after 24 hours
    public DateTime? CompletedAt { get; set; }
    public bool IsCompleted { get; set; }

    public User User { get; set; } = null!;
}