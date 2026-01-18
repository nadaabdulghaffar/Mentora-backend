using System;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;

namespace Mentora.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<UserDto>> RegisterInitialAsync(RegisterInitialRequest request);
        Task<ApiResponse<bool>> VerifyEmailAsync(VerifyEmailRequest request);
        Task<ApiResponse<bool>> ResendVerificationEmailAsync(string email);
        Task<ApiResponse<UserDto>> CompleteRegistrationAsync(CompleteRegistrationRequest request);
        Task<ApiResponse<bool>> CompleteMenteeProfileAsync(CompleteMenteeProfileRequest request);
        Task<ApiResponse<bool>> CompleteMentorProfileAsync(CompleteMentorProfileRequest request);
    }
}