
using Mentora.Application.DTOs.Auth;

public class AuthResponse
{


    public string AccessToken { get; set; } = null!;
    public string RefreshToken { get; set; } = null!;
    public int ExpiresIn { get; set; }
    public UserDto User { get; set; } = null!;


}