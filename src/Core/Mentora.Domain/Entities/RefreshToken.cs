using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentora.Domain.Entities
{
    public class RefreshToken
    {
        
            public Guid Id { get; private set; } = Guid.NewGuid();
            public Guid UserId { get; private set; }
            public string Token { get; private set; } = null!;
            public DateTime ExpiresAt { get; private set; }
            public DateTime CreatedAt { get; private set; } = DateTime.UtcNow;
            public bool IsRevoked { get; private set; }
            public DateTime? RevokedAt { get; private set; }

            public User User { get; private set; } = null!;

            private RefreshToken() { } // للـ EF Core

            public static RefreshToken Create(Guid userId, string token, TimeSpan validity)
            {
                return new RefreshToken
                {
                    UserId = userId,
                    Token = token,
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

