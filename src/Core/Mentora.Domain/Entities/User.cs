using System;
using System.Collections.Generic;
using Mentora.Domain.Enums; 

namespace Mentora.Domain.Entities
{
    public class User
    {
        public Guid UserId { get; set; }
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string FirstName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public UserRole Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? LastLogin { get; set; }
        public bool IsActive { get; set; }

        public MenteeProfile? MenteeProfile { get; set; }
        public MentorProfile? MentorProfile { get; set; }
        public ICollection<EmailVerificationToken> EmailVerificationTokens { get; set; }
    }
}