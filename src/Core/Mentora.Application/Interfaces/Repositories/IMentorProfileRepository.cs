using System;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;
using Mentora.Domain.Entities;

namespace Mentora.Application.Interfaces.Repositories;

public interface IMentorProfileRepository
{
    Task<MentorProfile?> GetByUserIdAsync(Guid userId);
    Task<MentorProfile> CreateAsync(MentorProfile profile);
    Task UpdateAsync(MentorProfile profile);
}
