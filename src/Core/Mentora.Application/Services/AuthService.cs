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
            await _unitOfWork.BeginTransactionAsync();
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

            // Create registration session
            var registrationSession = new RegistrationSession
            {
                SessionId = Guid.NewGuid(),
                UserId = user.UserId,
                SessionToken = GenerateSecureToken(),  // Different from email token
                CurrentStep = RegistrationStep.EmailVerificationPending,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddHours(24),
                IsCompleted = false
            };

            await _unitOfWork.RegistrationSessions.CreateAsync(registrationSession);

            await _unitOfWork.SaveChangesAsync();
            await _unitOfWork.CommitTransactionAsync();

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

        public async Task<ApiResponse<RegistrationFlowResponse>> VerifyEmailAsync(VerifyEmailRequest request)
        {
            var token = await _unitOfWork.EmailVerificationTokens.GetValidTokenAsync(request.Token);

            if (token == null)
            {
                return ApiResponse<RegistrationFlowResponse>.ErrorResponse("Invalid or expired verification token");
            }

            var user = await _unitOfWork.Users.GetByIdAsync(token.UserId);
            if (user == null)
            {
                return ApiResponse<RegistrationFlowResponse>.ErrorResponse("User not found");
            }

            // Mark email as verified
            user.IsEmailVerified = true;
            await _unitOfWork.Users.UpdateAsync(user);

            // Mark token as used
            await _unitOfWork.EmailVerificationTokens.MarkAsUsedAsync(token.TokenId);

            // Update registration session
            var session = await _unitOfWork.RegistrationSessions.GetByUserIdAsync(user.UserId);
            
            // Fix: If session is missing or expired, create a new one to allow user to proceed
            if (session == null || session.ExpiresAt < DateTime.UtcNow)
            {
                // Delete old invalid session if exists
                if (session != null) 
                {
                   await _unitOfWork.RegistrationSessions.DeleteAsync(session.SessionId);
                }

                session = new RegistrationSession
                {
                    SessionId = Guid.NewGuid(),
                    UserId = user.UserId,
                    SessionToken = GenerateSecureToken(),
                    CurrentStep = RegistrationStep.EmailVerified, // Set directly to verified
                    CreatedAt = DateTime.UtcNow,
                    ExpiresAt = DateTime.UtcNow.AddHours(24),
                    IsCompleted = false
                };
                await _unitOfWork.RegistrationSessions.CreateAsync(session);
            }
            else 
            {
                 session.CurrentStep = RegistrationStep.EmailVerified;
                 await _unitOfWork.RegistrationSessions.UpdateAsync(session);
            }

            await _unitOfWork.SaveChangesAsync();


            var response = new RegistrationFlowResponse
            {
                RegistrationToken = session.SessionToken,  //  Return this token
                CurrentStep = "EmailVerified",
                NextStep = "SelectRole",
                ExpiresAt = session.ExpiresAt,
                User = new UserBasicInfo
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = null
                }
            };
            return ApiResponse<RegistrationFlowResponse>.SuccessResponse(response, "Email verified successfully.");

        }

        public async Task<ApiResponse<bool>> ResendVerificationEmailAsync(ResendVerificationRequest request)
        {
            if (string.IsNullOrWhiteSpace(request?.Email))
                 return ApiResponse<bool>.ErrorResponse("Email is required");

            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email);
            if (user == null)
            {
                return ApiResponse<bool>.ErrorResponse("User not found");
            }

            if (user.IsEmailVerified)
            {
                 return ApiResponse<bool>.ErrorResponse("Email is already verified. Please login.");
            }
            
            if (user.IsActive)
            {
                 return ApiResponse<bool>.ErrorResponse("Account is already active. Please login.");
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

        public async Task<ApiResponse<RegistrationFlowResponse>> SelectRoleAsync(SelectRoleRequest request)
        {
            try
            {
                // Get user from registration token (not from request body)
                var session = await _unitOfWork.RegistrationSessions.GetByTokenAsync(request.RegistrationToken);

                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    return ApiResponse<RegistrationFlowResponse>.ErrorResponse(
                        "Invalid or expired registration session. Please start registration again."
                    );
                }

                if (session.CurrentStep != RegistrationStep.EmailVerified)
                {
                    return ApiResponse<RegistrationFlowResponse>.ErrorResponse(
                        "Please verify your email first."
                    );
                }

                var user = await _unitOfWork.Users.GetByIdAsync(session.UserId);
                if (user == null)
                {
                    return ApiResponse<RegistrationFlowResponse>.ErrorResponse("User not found");
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
                    return ApiResponse<RegistrationFlowResponse>.ErrorResponse(
                        "Invalid role. Must be 'mentee' or 'mentor'."
                    );
                }

                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                // Update session
                session.CurrentStep = RegistrationStep.RoleSelected;
                await _unitOfWork.RegistrationSessions.UpdateAsync(session);

                await _unitOfWork.SaveChangesAsync();

                var response = new RegistrationFlowResponse
                {
                    RegistrationToken = session.SessionToken,  //  Same token continues
                    CurrentStep = "RoleSelected",
                    NextStep = "CompleteProfile",
                    ExpiresAt = session.ExpiresAt,
                    User = new UserBasicInfo
                    {
                        UserId = user.UserId,
                        Email = user.Email,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        Role = user.Role.ToString()
                    }
                };

                return ApiResponse<RegistrationFlowResponse>.SuccessResponse(
                    response,
                    $"Role selected: {user.Role}. Please complete your profile."
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in SelectRoleAsync");
                return ApiResponse<RegistrationFlowResponse>.ErrorResponse($"Role selection failed: {ex.Message}");
            }
        }

        private string GenerateSecureToken()
        {
            var randomBytes = new byte[32];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes).Replace("+", "-").Replace("/", "_").TrimEnd('=');
        }

        public async Task<ApiResponse<RegistrationCompleteResponse>> CompleteMenteeProfileProgressiveAsync(
        CompleteMenteeProfileRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Get user from registration token
                var session = await _unitOfWork.RegistrationSessions.GetByTokenAsync(request.RegistrationToken);

                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse(
                        "Invalid or expired registration session."
                    );
                }

                if (session.CurrentStep != RegistrationStep.RoleSelected)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse(
                        "Please select your role first."
                    );
                }
                var user = await _unitOfWork.Users.GetByIdAsync(session.UserId);
                if (user == null)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("User not found");
                }

                if (user.Role != UserRole.Mentee)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("User is not a mentee");
                }

                // Check if profile already exists
                var existingProfile = await _unitOfWork.MenteeProfiles.GetByUserIdAsync(user.UserId);
                if (existingProfile != null)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("Profile already exists");
                }

                // Parse enums
                if (!Enum.TryParse<EducationStatus>(request.EducationStatus, true, out var educationStatus))
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse($"Invalid education status: {request.EducationStatus}");
                }

                if (!Enum.TryParse<ExperienceLevel>(request.ExperienceLevel, true, out var experienceLevel))
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse($"Invalid experience level: {request.ExperienceLevel}");
                }

                // Validate domain exists
                var domains = await _unitOfWork.Lookups.GetDomainsAsync();
                if (!domains.Any(d => d.DomainId == request.DomainId))
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("Invalid career field selected");
                }

                // Validate SubDomains exist
                foreach (var subDomainId in request.SubDomainIds)
                {
                    var subDomains = await _unitOfWork.Lookups.GetSubDomainsByDomainIdAsync(request.DomainId);
                    if (!subDomains.Any(s => s.SubDomainId == subDomainId))
                    {
                        return ApiResponse<RegistrationCompleteResponse>.ErrorResponse($"Invalid expertise area: {subDomainId}");
                    }
                }

                // Create mentee profile
                var profile = new MenteeProfile
                {
                    UserId = session.UserId,
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
                        UserId = session.UserId,
                        SubDomainId = subDomainId
                    };
                    profile.MenteeSubDomains.Add(menteeSubDomain);
                }

                // Add Technologies (Tools) - 1 to 5 selections
                if (request.TechnologyIds.Count < 1 || request.TechnologyIds.Count > 5)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("Please select between 1 and 5 tools");
                }

                foreach (var techId in request.TechnologyIds)
                {
                    var interest = new MenteeInterest
                    {
                        UserId = session.UserId,
                        TechnologyId = techId,
                        ExperienceLevel = ExperienceLevel.Beginner
                    };
                    profile.MenteeInterests.Add(interest);
                }
                //  Activate user account - registration complete!
                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                //  Mark registration session as complete
                session.CurrentStep = RegistrationStep.ProfileCompleted;
                session.IsCompleted = true;
                session.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.RegistrationSessions.UpdateAsync(session);

                //  Generate REAL authentication tokens (not registration token)
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var refreshTokenHash = _jwtService.HashToken(refreshToken);

                var tokenEntity = new RefreshToken
                {
                    TokenId = Guid.NewGuid(),
                    UserId = user.UserId,
                    TokenHash = refreshTokenHash,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.RefreshTokens.CreateAsync(tokenEntity);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName, "Mentee");

                var response = new RegistrationCompleteResponse
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    AccessToken = accessToken,      //  Real JWT token
                    RefreshToken = refreshToken,     //  Real refresh token
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };

                return ApiResponse<RegistrationCompleteResponse>.SuccessResponse(
                    response,
                    "Registration completed successfully! Welcome to Mentora!"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error in CompleteMenteeProfileProgressiveAsync");
                return ApiResponse<RegistrationCompleteResponse>.ErrorResponse(
                    $"Profile completion failed: {ex.Message}"
                );
            }

        }
        public async Task<ApiResponse<RegistrationCompleteResponse>> CompleteMentorProfileProgressiveAsync(
           CompleteMentorProfileRequest request)
        {
            await _unitOfWork.BeginTransactionAsync();

            try
            {
                // Get user from registration token
                var session = await _unitOfWork.RegistrationSessions.GetByTokenAsync(request.RegistrationToken);

                if (session == null || session.ExpiresAt < DateTime.UtcNow)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse(
                        "Invalid or expired registration session."
                    );
                }

                if (session.CurrentStep != RegistrationStep.RoleSelected)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse(
                        "Please select your role first."
                    );
                }

                var user = await _unitOfWork.Users.GetByIdAsync(session.UserId);
                if (user == null)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("User not found");
                }

                if (user.Role != UserRole.Mentor)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("User is not a mentor");
                }

                // Check if profile already exists
                var existingProfile = await _unitOfWork.MentorProfiles.GetByUserIdAsync(user.UserId);
                if (existingProfile != null)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("Profile already exists");
                }

                // Validate domain exists
                var domains = await _unitOfWork.Lookups.GetDomainsAsync();
                if (!domains.Any(d => d.DomainId == request.DomainId))
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("Invalid career field selected");
                }

                // Validate SubDomains exist
                foreach (var subDomainId in request.SubDomainIds)
                {
                    var subDomains = await _unitOfWork.Lookups.GetSubDomainsByDomainIdAsync(request.DomainId);
                    if (!subDomains.Any(s => s.SubDomainId == subDomainId))
                    {
                        return ApiResponse<RegistrationCompleteResponse>.ErrorResponse($"Invalid expertise area: {subDomainId}");
                    }
                }

                // Create mentor profile
                var profile = new MentorProfile
                {
                    UserId = user.UserId,
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
                        MentorId = user.UserId,
                        SubDomainId = subDomainId
                    };
                    profile.MentorSubDomains.Add(mentorSubDomain);
                }

                // Add Technologies (Tools) - 1 to 5 selections
                if (request.TechnologyIds.Count < 1 || request.TechnologyIds.Count > 5)
                {
                    return ApiResponse<RegistrationCompleteResponse>.ErrorResponse("Please select between 1 and 5 tools");
                }

                foreach (var techId in request.TechnologyIds)
                {
                    var expertise = new MentorExpertise
                    {
                        MentorId = user.UserId,
                        TechnologyId = techId
                    };
                    profile.MentorExpertises.Add(expertise);
                }

                //  Activate user account - registration complete!
                user.IsActive = true;
                user.UpdatedAt = DateTime.UtcNow;
                await _unitOfWork.Users.UpdateAsync(user);

                //  Mark registration session as complete
                session.CurrentStep = RegistrationStep.ProfileCompleted;
                session.IsCompleted = true;
                session.CompletedAt = DateTime.UtcNow;
                await _unitOfWork.RegistrationSessions.UpdateAsync(session);

                //  Generate REAL authentication tokens (not registration token)
                var accessToken = _jwtService.GenerateAccessToken(user);
                var refreshToken = _jwtService.GenerateRefreshToken();
                var refreshTokenHash = _jwtService.HashToken(refreshToken);

                var tokenEntity = new RefreshToken
                {
                    TokenId = Guid.NewGuid(),
                    UserId = user.UserId,
                    TokenHash = refreshTokenHash,
                    ExpiresAt = DateTime.UtcNow.AddDays(7),
                    CreatedAt = DateTime.UtcNow
                };

                await _unitOfWork.RefreshTokens.CreateAsync(tokenEntity);

                await _unitOfWork.SaveChangesAsync();
                await _unitOfWork.CommitTransactionAsync();

                // Send welcome email
                await _emailService.SendWelcomeEmailAsync(user.Email, user.FirstName, "Mentee");

                var response = new RegistrationCompleteResponse
                {
                    UserId = user.UserId,
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Role = user.Role.ToString(),
                    AccessToken = accessToken,      //  Real JWT token
                    RefreshToken = refreshToken,     //  Real refresh token
                    ExpiresAt = DateTime.UtcNow.AddMinutes(60)
                };

                return ApiResponse<RegistrationCompleteResponse>.SuccessResponse(
                    response,
                    "Registration completed successfully! Welcome to Mentora!"
                );
            }
            catch (Exception ex)
            {
                await _unitOfWork.RollbackTransactionAsync();
                _logger.LogError(ex, "Error in CompleteMenteeProfileProgressiveAsync");
                return ApiResponse<RegistrationCompleteResponse>.ErrorResponse(
                    $"Profile completion failed: {ex.Message}"
                );
            }

        }



          public async Task<ApiResponse<AuthResponse>> ExternalLoginAsync(string email, string firstName, string lastName, string provider, bool rememberMe)
        {
            
            var user = await _unitOfWork.Users.GetByEmailAsync(email.Trim().ToLower());

            if (user == null)
            {
                user = new User
                {
                    UserId = Guid.NewGuid(),
                    Email = email.Trim().ToLower(),
                    FirstName = firstName,
                    LastName = lastName,
                    Role = UserRole.Mentee, 
                    IsActive = true,
                    IsEmailVerified = true,
                    CreatedAt = DateTime.UtcNow,
                };

                await _unitOfWork.Users.CreateAsync(user);
            }

          //  if (!user.IsActive)
             //   return ApiResponse<AuthResponse>.ErrorResponse("Account Is Not Active");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshTokenStr = _jwtService.GenerateRefreshToken();
            var expiryTimeSpan = rememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromDays(7);

            var refreshToken = RefreshToken.Create(user.UserId, refreshTokenStr, expiryTimeSpan);

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


        public async Task<ApiResponse<AuthResponse>> LoginAsync(LoginRequest request)
        {
            if (request == null || string.IsNullOrEmpty(request.Email))
                return ApiResponse<AuthResponse>.ErrorResponse("Email Is Required");

            var user = await _unitOfWork.Users.GetByEmailAsync(request.Email.Trim().ToLower());

            if (user == null)
                return ApiResponse<AuthResponse>.ErrorResponse("Email Or Password Is Wrong");


            bool isPasswordValid = _passwordHasher.VerifyPassword(request.Password, user.PasswordHash);

            if (!isPasswordValid)
                return ApiResponse<AuthResponse>.ErrorResponse("Email Or Password Is Wrong");


            //if (!user.IsActive)
            //    return ApiResponse<AuthResponse>.ErrorResponse("Account Is Not Active");

            var accessToken = _jwtService.GenerateAccessToken(user);
            var refreshTokenStr = _jwtService.GenerateRefreshToken();

            var expiryTimeSpan = request.RememberMe ? TimeSpan.FromDays(30) : TimeSpan.FromDays(7);
            var refreshToken = RefreshToken.Create(user.UserId, refreshTokenStr, expiryTimeSpan);

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


        public async Task<ApiResponse<AuthResponse>> RefreshTokenAsync(RefreshTokenRequest request)
        {
            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);


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

        public async Task<ApiResponse<bool>> LogoutAsync(RefreshTokenRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.RefreshToken))
            {
                return ApiResponse<bool>.SuccessResponse(true);
            }

            var storedToken = await _unitOfWork.RefreshTokens.GetByTokenAsync(request.RefreshToken);

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
