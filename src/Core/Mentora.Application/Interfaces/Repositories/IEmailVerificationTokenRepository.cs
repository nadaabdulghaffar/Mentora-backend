namespace Mentora.Application.Interfaces.Repositories;

public interface IEmailVerificationTokenRepository
{
    Task<EmailVerificationToken> CreateAsync(EmailVerificationToken token);
    Task<EmailVerificationToken?> GetValidTokenAsync(string token);
    Task MarkAsUsedAsync(Guid tokenId);
    Task DeleteExpiredTokensAsync(Guid userId);
}