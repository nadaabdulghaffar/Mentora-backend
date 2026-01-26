namespace Mentora.Persistence.Repositories;

using Microsoft.EntityFrameworkCore;
using Mentora.Application.Interfaces.Repositories;
using Mentora.Application.Interfaces;
using Mentora.Domain.Entities;

public class RegistrationSessionRepository : IRegistrationSessionRepository
{
    private readonly ApplicationDbContext _context;

    public RegistrationSessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<RegistrationSession> CreateAsync(RegistrationSession session)
    {
        await _context.RegistrationSessions.AddAsync(session);
        return session;
    }

    public async Task<RegistrationSession?> GetByTokenAsync(string token)
    {
        return await _context.RegistrationSessions
            .Include(s => s.User)
            .FirstOrDefaultAsync(s =>
                s.SessionToken == token &&
                s.ExpiresAt > DateTime.UtcNow &&
                !s.IsCompleted
            );
    }

    public async Task<RegistrationSession?> GetByUserIdAsync(Guid userId)
    {
        return await _context.RegistrationSessions
            .Include(s => s.User)
            .Where(s => s.UserId == userId && !s.IsCompleted)
            .OrderByDescending(s => s.CreatedAt)
            .FirstOrDefaultAsync();
    }

    public Task UpdateAsync(RegistrationSession session)
    {
        _context.RegistrationSessions.Update(session);
        return Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid sessionId)
    {
        var session = await _context.RegistrationSessions.FindAsync(sessionId);
        if (session != null)
        {
            _context.RegistrationSessions.Remove(session);
        }
    }

    public async Task DeleteExpiredSessionsAsync()
    {
        var expiredSessions = await _context.RegistrationSessions
            .Where(s => s.ExpiresAt <= DateTime.UtcNow || s.IsCompleted)
            .ToListAsync();

        _context.RegistrationSessions.RemoveRange(expiredSessions);
    }
}
