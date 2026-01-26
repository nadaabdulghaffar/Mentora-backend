using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentora.Domain.Entities
{
    public class RefreshToken
    {

        public Guid TokenId { get; set; } = Guid.NewGuid(); // Renamed from Id
        public Guid UserId { get; set; }
        public string TokenHash { get; set; } = null!; // Renamed from Token
        public DateTime ExpiresAt { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRevoked { get; set; }
        public DateTime? RevokedAt { get; set; }

        public User User { get; set; } = null!;

        public RefreshToken() { } // made public for object initializer

        public static RefreshToken Create(Guid userId, string tokenHash, TimeSpan validity)
        {
            return new RefreshToken
            {
                UserId = userId,
                TokenHash = tokenHash,
                ExpiresAt = DateTime.UtcNow.Add(validity)
            };
        }

        public void Revoke()
        {
            if (!IsRevoked)
            {
                IsRevoked = true;
                RevokedAt = DateTime.UtcNow;
            }
        }

        public bool IsValid => !IsRevoked && ExpiresAt > DateTime.UtcNow;
    }
}