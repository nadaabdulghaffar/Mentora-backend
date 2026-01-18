namespace Mentora.Persistence;

using Microsoft.EntityFrameworkCore.Storage;
using Mentora.Application.Interfaces;
using Mentora.Application.Interfaces.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
        Users = new UserRepository(_context);
        MenteeProfiles = new MenteeProfileRepository(_context);
        MentorProfiles = new MentorProfileRepository(_context);
        EmailVerificationTokens = new EmailVerificationTokenRepository(_context);
        RefreshTokens = new RefreshTokenRepository(_context);
        PasswordResetTokens = new PasswordResetTokenRepository(_context);
        Lookups = new LookupRepository(_context);
    }

    public IUserRepository Users { get; }
    public IMenteeProfileRepository MenteeProfiles { get; }
    public IMentorProfileRepository MentorProfiles { get; }
    public IEmailVerificationTokenRepository EmailVerificationTokens { get; }
    public IRefreshTokenRepository RefreshTokens { get; }
    public IPasswordResetTokenRepository PasswordResetTokens { get; }
    public ILookupRepository Lookups { get; }

    public async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        return await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public void Dispose()
    {
        _transaction?.Dispose();
        _context.Dispose();
    }
}