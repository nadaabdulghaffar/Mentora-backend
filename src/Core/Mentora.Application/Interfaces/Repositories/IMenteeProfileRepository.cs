using System;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;
using Mentora.Domain.Entities;

namespace Mentora.Application.Interfaces.Repositories;

public interface IMenteeProfileRepository
{
    Task<MenteeProfile?> GetByUserIdAsync(Guid userId);
    Task<MenteeProfile> CreateAsync(MenteeProfile profile);
    Task UpdateAsync(MenteeProfile profile);
}