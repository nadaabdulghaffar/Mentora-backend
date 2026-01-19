using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;
using Mentora.Application.Interfaces;
using Mentora.Domain.Entities;
using Mentora.Domain.Enums;
using System.Security.Cryptography;


namespace Mentora.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService ;

        public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IEmailService emailService,
         IJwtService jwtService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _jwtService = jwtService;
        }
        public async Task<ApiResponse<UserDto>> RegisterInitialAsync(RegisterInitialRequest request)
        {

            // Check if email already exists
            if (await _unitOfWork.Users.EmailExistsAsync(request.Email))
            {
                return ApiResponse<UserDto>.ErrorResponse("Email already registered");
            }

            // Create user
            var user = new User
            {
                UserId = Guid.NewGuid(),
                Email = request.Email.ToLower(),
                PasswordHash = _passwordHasher.HashPassword(request.Password),
                FirstName = request.FirstName,
                LastName = request.LastName,
                Role = UserRole.Mentee, // Default, will be updated later
                CreatedAt = DateTime.UtcNow,
                IsActive = false // Activated after email verification
            };

            await _unitOfWork.Users.CreateAsync(user);

            // Generate email verification token
            var verificationToken = GenerateSecureToken();
            var emailToken = new EmailVerificationToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = verificationToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                CreatedAt = DateTime.UtcNow
            };
            await _unitOfWork.EmailVerificationTokens.CreateAsync(emailToken);
            await _unitOfWork.SaveChangesAsync();

            // Send verification email
            await _emailService.SendVerificationEmailAsync(user.Email, user.FirstName, verificationToken);

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                IsEmailVerified = false,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };
            
            return ApiResponse<UserDto>.SuccessResponse(userDto);
        }

        public async Task<ApiResponse<bool>> VerifyEmailAsync(VerifyEmailRequest request)
        {
            var token = await _unitOfWork.EmailVerificationTokens.GetValidTokenAsync(request.Token);

            if (token == null)
            {
                return ApiResponse<bool>.ErrorResponse("Invalid or expired verification token");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResponse("User not found");
            }

            // Mark token as used
            await _unitOfWork.EmailVerificationTokens.MarkAsUsedAsync(token.TokenId);

            // Delete any other expired tokens for this user
            await _unitOfWork.EmailVerificationTokens.DeleteExpiredTokensAsync(user.UserId);

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Email verified successfully. Please complete your registration.");
        }

        public async Task<ApiResponse<bool>> ResendVerificationEmailAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResponse("User not found");
            }

            // Delete old tokens
            await _unitOfWork.EmailVerificationTokens.DeleteExpiredTokensAsync(user.UserId);

            // Generate new token
            var verificationToken = GenerateSecureToken();
            var emailToken = new EmailVerificationToken
            {
                TokenId = Guid.NewGuid(),
                UserId = user.UserId,
                Token = verificationToken,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                CreatedAt = DateTime.UtcNow
            };

            await _unitOfWork.EmailVerificationTokens.CreateAsync(emailToken);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendVerificationEmailAsync(user.Email, user.FirstName, verificationToken);

            return ApiResponse<bool>.SuccessResponse(true, "Verification email resent successfully");
        }

        public async Task<ApiResponse<UserDto>> CompleteRegistrationAsync(CompleteRegistrationRequest request)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
            if (user == null)
            {
                return ApiResponse<UserDto>.ErrorResponse("User not found");
            }

            // Update user role
            if (request.Role.ToLower() == "mentee")
            {
                user.Role = UserRole.Mentee;
            }
            else if (request.Role.ToLower() == "mentor")
            {
                user.Role = UserRole.Mentor;
            }
            else
            {
                return ApiResponse<UserDto>.ErrorResponse("Invalid role specified");
            }

            user.UpdatedAt = DateTime.UtcNow;
            await _unitOfWork.Users.UpdateAsync(user);
            await _unitOfWork.SaveChangesAsync();

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                IsEmailVerified = true,
                IsActive = user.IsActive,
                CreatedAt = user.CreatedAt
            };

            return ApiResponse<UserDto>.SuccessResponse(userDto, "Role selected successfully");
        }
        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public Task<ApiResponse<bool>> CompleteMenteeProfileAsync(CompleteMenteeProfileRequest request)
        {
            throw new NotImplementedException();
        }

        public Task<ApiResponse<bool>> CompleteMentorProfileAsync(CompleteMentorProfileRequest request)
        {
            throw new NotImplementedException();
        }
        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Email))
                return ApiResponse<AuthResponse>.ErrorResponse("Email Is Required"); 

            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.ToLower());

            if (user == null || !_passwordHasher.VerifyPassword(user.PasswordHash, request.Password))
                return ApiResponse<AuthResponse>.ErrorResponse("Email Or Password Is Wrong");

            if (!user.IsActive)
                return ApiResponse<AuthResponse>.ErrorResponse("Account Is Not Active");

           
            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshTokenStr = _jwtService.GenerateRefreshToken();
            var refreshToken = RefreshToken.Create(user.UserId, refreshTokenStr, TimeSpan.FromDays(7));

            await _unitOfWork.RefreshTokens.CreateAsync(refreshToken);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponse
            {
                AccessToken = accessToken,
                RefreshToken = refreshTokenStr,
                ExpiresIn = 3600,
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString()
                }
            };

            return ApiResponse<AuthResponse>.SuccessResponse(response);
        }


        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

           
            if (storedToken == null || !storedToken.IsValid)
            {
                return ApiResponse<AuthResponse>.ErrorResponse("Invalid or expired token. Please log in again.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(storedToken.UserId);
            if (user == null || !user.IsActive)
            {
                return ApiResponse<AuthResponse>.ErrorResponse("User account not found or inactive.");
            }

            var newAccessToken = _jwtService.GenerateAccessToken(user);
            var newRefreshTokenStr = _jwtService.GenerateRefreshToken();
            var newRefreshToken = RefreshToken.Create(user.UserId, newRefreshTokenStr, TimeSpan.FromDays(7));

            storedToken.Revoke();
            await _unitOfWork.RefreshTokens.CreateAsync(newRefreshToken);
            await _unitOfWork.SaveChangesAsync();

            var response = new AuthResponse
            {
                AccessToken = newAccessToken,
                RefreshToken = newRefreshTokenStr,
                ExpiresIn = 3600, 
                User = new UserDto
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    IsEmailVerified = user.IsEmailVerified,
                    IsActive = user.IsActive
                }
            };

            return ApiResponse<AuthResponse>.SuccessResponse(response);
        }



        public async Task<ApiResponse<bool>> LogoutAsync(string refreshToken)
        {
            if (string.IsNullOrWhiteSpace(refreshToken))
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }

            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(refreshToken);

            if (storedToken != null && storedToken.IsValid)
            {
                storedToken.Revoke();
                await _unitOfWork.SaveChangesAsync();
            }

            return ApiResponse<bool>.SuccessResponse(true, "Logged out successfully.");
        }
        public async Task<ApiResponse<bool>> ForgotPasswordAsync(string email)
        {
            var user = await _unitOfWork.Users.GetByEmailAsync(email);

            if (user == null)
            {
                return ApiResponse<bool>.SuccessResponse(true, "If an account with this email exists, a password reset link has been sent.");
            }

            var resetTokenString = GenerateSecureToken();

            var passwordResetToken = new PasswordResetToken
            {
                UserId = user.UserId,
                Token = resetTokenString,
                ExpiresAt = DateTime.UtcNow.AddHours(1)
            };

            await _unitOfWork.PasswordResetTokens.CreateAsync(passwordResetToken);
            await _unitOfWork.SaveChangesAsync();

            await _emailService.SendPasswordResetEmailAsync(user.Email, user.FirstName, resetTokenString);

            return ApiResponse<bool>.SuccessResponse(true, "If an account with this email exists, a password reset link has been sent.");
        }
        public async Task<ApiResponse<bool>> ResetPasswordAsync(ResetPasswordRequest request)
        {
            var storedToken = await _unitOfWork.PasswordResetTokens.GetActiveTokenAsync(request.Token);

            if (storedToken == null)
            {
                return ApiResponse<bool>.ErrorResponse("Invalid or expired password reset token.");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResponse("An error occurred: User not found.");
            }

            user.PasswordHash = _passwordHasher.HashPassword(request.NewPassword);
            user.UpdatedAt = DateTime.UtcNow;

            storedToken.UsedAt = DateTime.UtcNow;

            await _unitOfWork.SaveChangesAsync();

            return ApiResponse<bool>.SuccessResponse(true, "Your password has been reset successfully. You can now log in.");
        }

        public async Task<ApiResponse<UserDto>> GetCurrentUserAsync(Guid userId)
        {
            var user = await _unitOfWork.Users.GetByIdAsync(userId);

            if (user == null)
            {
                return ApiResponse<UserDto>.ErrorResponse("User not found.");
            }

            var userDto = new UserDto
            {
                UserId = user.UserId,
                Email = user.Email,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Role = user.Role.ToString(),
                IsEmailVerified = user.IsEmailVerified,
                IsActive = user.IsActive
            };

            return ApiResponse<UserDto>.SuccessResponse(userDto);
        }

    }
}