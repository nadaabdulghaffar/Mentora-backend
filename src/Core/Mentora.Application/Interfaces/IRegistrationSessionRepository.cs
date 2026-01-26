namespace Mentora.Application.Interfaces;

using Mentora.Domain.Entities;

public interface IRegistrationSessionRepository
{
    Task<RegistrationSession> CreateAsync(RegistrationSession session);
    Task<RegistrationSession?> GetByTokenAsync(string token);
    Task<RegistrationSession?> GetByUserIdAsync(Guid userId);
    Task UpdateAsync(RegistrationSession session);
    Task DeleteAsync(Guid sessionId);
    Task DeleteExpiredSessionsAsync();
}