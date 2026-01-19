using Mentora.Domain.Entities;

public interface IRefreshTokenRepository
{
    Task CreateAsync(RefreshToken token);
 
   Task<RefreshToken?> GetByTokenAsync(string token);
    Task RevokeTokenAsync(string token);
    Task RevokeAllUserTokensAsync(Guid userId);
  

}