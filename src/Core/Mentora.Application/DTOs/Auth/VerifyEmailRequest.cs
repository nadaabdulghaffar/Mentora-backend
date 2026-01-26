namespace Mentora.Application.DTOs.Auth;

public class VerifyEmailRequest
{
    public string Token { get; set; } = null!;
    public string Email { get; set; }
}
