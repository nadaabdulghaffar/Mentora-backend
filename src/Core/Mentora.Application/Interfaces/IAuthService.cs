using System;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;

namespace Mentora.Application.Interfaces
{
    public interface IAuthService
    {
        Task<ApiResponse<UserDto>> RegisterInitialAsync(RegisterInitialRequest request);
        public Task<ApiResponse<RegistrationFlowResponse>> VerifyEmailAsync(VerifyEmailRequest request);
        public Task<ApiResponse<RegistrationFlowResponse>> SelectRoleAsync(SelectRoleRequest request);
        public Task<ApiResponse<RegistrationCompleteResponse>> CompleteMenteeProfileProgressiveAsync(
        CompleteMenteeProfileRequest request);
        public Task<ApiResponse<RegistrationCompleteResponse>> CompleteMentorProfileProgressiveAsync(
     CompleteMentorProfileRequest request);
        Task<ApiResponse<bool>> ResendVerificationEmailAsync(ResendVerificationRequest request);
        Task<ApiResponse<AuthResponse>> ExternalLoginAsync(string email, string firstName, string lastName, string provider,bool remmberMe);
        Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request);
        Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request);
        Task<ApiResponse<bool>> LogoutAsync(RefreshTokenRequest request);
        Task<ApiResponse<bool>> ForgotPasswordAsync(string email);
        Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request);
        Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId);
    }
}