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

using Microsoft.Extensions.Logging;

namespace Mentora.Application.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPasswordHasher _passwordHasher;
        private readonly IEmailService _emailService;
        private readonly IJwtService _jwtService;
        private readonly ILogger<AuthService> _logger;

        public AuthService(
                IUnitOfWork unitOfWork,
                IPasswordHasher passwordHasher,
                IEmailService emailService,
                IJwtService jwtService,
                ILogger<AuthService> logger)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
            _jwtService = jwtService;
            _logger = logger;
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
                UpdatedAt = null,
                IsActive = false // Activated after email verification
            };

            await _unitOfWork.Users.CreateAsync(user);

            // Save user FIRST before creating token
            await _unitOfWork.SaveChangesAsync();

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

            return ApiResponse<UserDto>.SuccessResponse(userDto, "Registration successful. Please verify your email.");
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

        public async Task<ApiResponse<bool>> CompleteMenteeProfileAsync(
        Guid userId,
        CompleteMenteeProfileRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("User not found");
                }

                if (user.Role != UserRole.Mentee)
                {
                    return ApiResponse<bool>.ErrorResponse("User is not a mentee");
                }

                // Check if profile already exists
                var existingProfile = await _unitOfWork.MenteeProfiles.GetByUserIdAsync(userId);
                if (existingProfile != null)
                {
                    return ApiResponse<bool>.ErrorResponse("Profile already exists");
                }

                // Parse enums
                if (!Enum.TryParse<EducationStatus>(request.EducationStatus, true, out var educationStatus))
                {
                    return ApiResponse<bool>.ErrorResponse($"Invalid education status: {request.EducationStatus}");
                }

                if (!Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, true, out var experienceLevel))
                {
                    return ApiResponse<bool>.ErrorResponse($"Invalid experience level: {request.ExperienceLevel}");
                }

                // Validate domain exists
                var domains = await _unitOfWork.Lookups.GetDomainsAsync();
                if (!domains.Any(d => d.DomainId == request.DomainId))
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid career field selected");
                }

                // Validate SubDomains exist
                foreach (var subDomainId in request.SubDomainIds)
                {
                    var subDomains = await _unitOfWork.Lookups.GetSubDomainsByDomainIdAsync(request.DomainId);
                    if (!subDomains.Any(s => s.SubDomainId == subDomainId))
                    {
                        return ApiResponse<bool>.ErrorResponse($"Invalid expertise area: {subDomainId}");
                    }
                }

                // Create mentee profile
                var profile = new MenteeProfile
                {
                    UserId = userId,
                    DomainId = request.DomainId,
                    EducationStatus = educationStatus,
                    CurrentLevel = experienceLevel,
                    CareerGoalId = request.CareerGoalId,
                    LearningStyleId = request.LearningStyleId,
                    Bio = request.Bio,
                    CountryCode = request.CountryCode,
                    IsEmailVerified = true
                };

                await _unitOfWork.MenteeProfiles.CreateAsync(profile);

                // Add SubDomains (Relevant expertise)
                foreach (var subDomainId in request.SubDomainIds)
                {
                    var menteeSubDomain = new MenteeSubDomain
                    {
                        UserId = userId,
                        SubDomainId = subDomainId
                    };
                    profile.MenteeSubDomains.Add(menteeSubDomain);
                }

                // Add Technologies (Tools) - 1 to 5 selections
                if (request.TechnologyIds.Count < 1 || request.TechnologyIds.Count > 5)
                {
                    return ApiResponse<bool>.ErrorResponse("Please select between 1 and 5 tools");
                }

                foreach (var techId in request.TechnologyIds)
                {
                    var interest = new MenteeInterest
                    {
                        UserId = userId,
                        TechnologyId = techId,
                        ExperienceLevel = ExperienceLevel.Beginner
                    };
                    profile.MenteeInterests.Add(interest);
                }

                // Activate user account
                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName, "Mentee");

                return ApiResponse<bool>.SuccessResponse(true, "Mentee profile completed successfully");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error completing mentee profile for user {UserId}", userId);
                return ApiResponse<bool>.ErrorResponse($"Error completing profile: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CompleteMentorProfileAsync(
            Guid userId,
            CompleteMentorProfileRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(userId);
                if (user == null)
                {
                    return ApiResponse<bool>.ErrorResponse("User not found");
                }

                if (user.Role != UserRole.Mentor)
                {
                    return ApiResponse<bool>.ErrorResponse("User is not a mentor");
                }

                // Check if profile already exists
                var existingProfile = await _unitOfWork.MentorProfiles.GetByUserIdAsync(userId);
                if (existingProfile != null)
                {
                    return ApiResponse<bool>.ErrorResponse("Profile already exists");
                }

                // Validate domain exists
                var domains = await _unitOfWork.Lookups.GetDomainsAsync();
                if (!domains.Any(d => d.DomainId == request.DomainId))
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid career field selected");
                }

                // Validate SubDomains exist
                foreach (var subDomainId in request.SubDomainIds)
                {
                    var subDomains = await _unitOfWork.Lookups.GetSubDomainsByDomainIdAsync(request.DomainId);
                    if (!subDomains.Any(s => s.SubDomainId == subDomainId))
                    {
                        return ApiResponse<bool>.ErrorResponse($"Invalid expertise area: {subDomainId}");
                    }
                }

                // Create mentor profile
                var profile = new MentorProfile
                {
                    UserId = userId,
                    DomainId = request.DomainId,
                    YearsOfExperience = request.YearsOfExperience,
                    LinkedInUrl = request.LinkedInUrl,
                    Bio = request.Bio,
                    CvUrl = request.CvUrl,  // CV URL from file upload
                    CountryCode = request.CountryCode,
                    IsEmailVerified = true,
                    IsVerified = false, // Requires admin approval
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.MentorProfiles.CreateAsync(profile);

                // Add SubDomains (Relevant expertise)
                foreach (var subDomainId in request.SubDomainIds)
                {
                    var mentorSubDomain = new MentorSubDomain
                    {
                        MentorId = userId,
                        SubDomainId = subDomainId
                    };
                    profile.MentorSubDomains.Add(mentorSubDomain);
                }

                // Add Technologies (Tools) - 1 to 5 selections
                if (request.TechnologyIds.Count < 1 || request.TechnologyIds.Count > 5)
                {
                    return ApiResponse<bool>.ErrorResponse("Please select between 1 and 5 tools");
                }

                foreach (var techId in request.TechnologyIds)
                {
                    var expertise = new MentorExpertise
                    {
                        MentorId = userId,
                        TechnologyId = techId
                    };
                    profile.MentorExpertises.Add(expertise);
                }

                // Activate user account (but mentor needs admin verification)
                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName, "Mentor");

                return ApiResponse<bool>.SuccessResponse(
                    true,
                    "Mentor profile completed successfully. Your profile is pending admin verification."
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error completing mentor profile for user {UserId}", userId);
                return ApiResponse<bool>.ErrorResponse($"Error completing profile: {ex.Message}");
            }
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
