using Mentora.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mentora.Application.Interfaces.Repositories
{
    public interface IPasswordResetTokenRepository
    {
        Task CreateAsync(PasswordResetToken token);
        Task<PasswordResetToken?> GetActiveTokenAsync(string token);
    }
}
