namespace Mentora.Application.Interfaces

public interface IUnitOfWork : IDisposable
{
    IUserRepository Users { get; }
    IMenteeProfileRepository MenteeProfiles { get; }
    IMentorProfileRepository MentorProfiles { get; }
    IEmailVerificationTokenRepository EmailVerificationTokens { get; }
    ILookupRepository Lookups { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}