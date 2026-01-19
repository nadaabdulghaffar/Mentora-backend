using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mentora.Application.Interfaces;
using Mentora.Application.Interfaces.Repositories;
using Mentora.Domain.Entities;


namespace Mentora.Persistence.Repositories;

public class MenteeProfileRepository : IMenteeProfileRepository
{
    private readonly ApplicationDbContext _context;

    public MenteeProfileRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<MenteeProfile?> GetByUserIdAsync(Guid userId)
    {
        return await _context.MenteeProfiles
            .Include(p => p.Domain)
            .Include(p => p.CareerGoal)
            .Include(p => p.LearningStyle)
            .Include(p => p.MenteeInterests)
                .ThenInclude(i => i.Technology)
            .FirstOrDefaultAsync(p => p.UserId == userId);
    }

    public async Task<MenteeProfile> CreateAsync(MenteeProfile profile)
    {
        await _context.MenteeProfiles.AddAsync(profile);
        return profile;
    }

    public Task UpdateAsync(MenteeProfile profile)
    {
        _context.MenteeProfiles.Update(profile);
        return Task.CompletedTask;
    }
}