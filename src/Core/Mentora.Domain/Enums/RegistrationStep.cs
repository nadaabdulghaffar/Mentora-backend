namespace Mentora.Domain.Enums;

public enum RegistrationStep
{
    EmailVerificationPending = 1,  // User created, waiting for email verification
    EmailVerified = 2,              // Email verified, needs to choose role
    RoleSelected = 3,               // Role chosen, needs to complete profile
    ProfileCompleted = 4            // Profile completed, registration done
}