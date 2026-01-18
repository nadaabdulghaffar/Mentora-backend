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

        public AuthService(
        IUnitOfWork unitOfWork,
        IPasswordHasher passwordHasher,
        IEmailService emailService)
        {
            _unitOfWork = unitOfWork;
            _passwordHasher = passwordHasher;
            _emailService = emailService;
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

        public async Task<ApiResponse<bool>> CompleteMenteeProfileAsync(CompleteMenteeProfileRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null || user.Role != UserRole.Mentee)
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid user or role");
                }

                // Parse enums
                if (!Enum.TryParse<EducationStatus>(request.EducationStatus, true, out var educationStatus))
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid education status");
                }

                if (!Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, true, out var experienceLevel))
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid experience level");
                }

                // Create mentee profile
                var profile = new MenteeProfile
                {
                    UserId = user.UserId,
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

                // Add interests
                foreach (var techId in request.TechnologyIds)
                {
                    var interest = new MenteeInterest
                    {
                        UserId = user.UserId,
                        TechnologyId = techId,
                        ExperienceLevel = ExperienceLevel.Beginner // Default
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
                return ApiResponse<bool>.ErrorResponse($"Error completing profile: {ex.Message}");
            }
        }

        public async Task<ApiResponse<bool>> CompleteMentorProfileAsync(CompleteMentorProfileRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(request.UserId);
                if (user == null || user.Role != UserRole.Mentor)
                {
                    return ApiResponse<bool>.ErrorResponse("Invalid user or role");
                }

                // Create mentor profile
                var profile = new MentorProfile
                {
                    UserId = user.UserId,
                    DomainId = request.DomainId,
                    YearsOfExperience = request.YearsOfExperience,
                    LinkedInUrl = request.LinkedInUrl,
                    Bio = request.Bio,
                    CvUrl = request.CvUrl,
                    CountryCode = request.CountryCode,
                    IsEmailVerified = true,
                    IsVerified = false, // Requires admin approval
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.MentorProfiles.CreateAsync(profile);

                // Add expertise
                foreach (var techId in request.TechnologyIds)
                {
                    var expertise = new MentorExpertise
                    {
                        MentorId = user.UserId,
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

                return ApiResponse<bool>.SuccessResponse(true, "Mentor profile completed. Pending admin verification.");
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                return ApiResponse<bool>.ErrorResponse($"Error completing profile: {ex.Message}");
            }
        }

    }
}