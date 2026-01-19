using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mentora.Application.Interfaces;
using Mentora.Application.Interfaces.Repositories;
using Mentora.Domain.Entities;

namespace Mentora.Persistence.Repositories;

public class MentorProfileRepository : IMentorProfileRepository
{
    private readonly ApplicationDbContext _context;

    public MentorProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MentorProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.MentorProfiles
            .Include(p => p.Domain)
            .Include(p => p.MentorExpertises)
                .ThenInclude(e => e.Technology)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<MentorProfile> CreateAsync(MentorProfile profile)
    {
        await _context.MentorProfiles.AddAsync(profile);
        return profile;
    }

    public Task UpdateAsync(MentorProfile profile)
    {
        _context.MentorProfiles.Update(profile);
        return Task.CompletedTask;
    }
}