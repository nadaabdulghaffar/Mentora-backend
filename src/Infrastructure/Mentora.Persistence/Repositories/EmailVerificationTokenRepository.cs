using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mentora.Application.Interfaces;
using Mentora.Application.Interfaces.Repositories;
using Mentora.Domain.Entities;


namespace Mentora.Persistence.Repositories;

public class EmailVerificationTokenRepository : IEmailVerificationTokenRepository
{
    private readonly ApplicationDbContext _context;

    public EmailVerificationTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token)
    {
        await _context.EmailVerificationTokens.AddAsync(token);
        return token;
    }

    public async Task<EmailVerificationToken?> GetValidTokenAsync(string token)
    {
        return await _context.EmailVerificationTokens
            .FirstOrDefaultAsync(t => t.Token == token
                && t.ExpiresAt > DateTime.UtcNow
                && t.UsedAt == null);
    }

    public async Task MarkAsUsedAsync(Guid tokenId)
    {
        var token = await _context.EmailVerificationTokens.FindAsync(tokenId);
        if (token != null)
        {
            token.UsedAt = DateTime.UtcNow;
            _context.EmailVerificationTokens.Update(token);
        }
    }

    public async Task DeleteExpiredTokensAsync(Guid userId)
    {
        var expiredTokens = await _context.EmailVerificationTokens
            .Where(t => t.UserId == userId && (t.ExpiresAt <= DateTime.UtcNow || t.UsedAt != null))
            .ToListAsync();

        _context.EmailVerificationTokens.RemoveRange(expiredTokens);
    }

   
}