using Microsoft.EntityFrameworkCore;
using Mentora.Application.Interfaces.Repositories;
using Mentora.Domain.Entities;

namespace Mentora.Persistence.Repositories;

public class RefreshTokenRepository : IRefreshTokenRepository
{
    private readonly ApplicationDbContext _context;

    public RefreshTokenRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task CreateAsync(RefreshToken token)
    {
        await _context.RefreshTokens.AddAsync(token);
    }

    public async Task<RefreshToken?> GetByTokenAsync(string token)
    {
        return await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == token);
    }

    public async Task RevokeTokenAsync(string token)
    {
        var refreshToken = await _context.RefreshTokens
            .FirstOrDefaultAsync(t => t.TokenHash == token);

        if (refreshToken != null)
        {
            refreshToken.Revoke();
        }
    }

    public async Task RevokeAllUserTokensAsync(Guid userId)
    {
        var tokens = await _context.RefreshTokens
            .Where(t => t.UserId == userId && !t.IsRevoked)
            .ToListAsync();

        foreach (var t in tokens)
        {
            t.Revoke();
        }


    }
}