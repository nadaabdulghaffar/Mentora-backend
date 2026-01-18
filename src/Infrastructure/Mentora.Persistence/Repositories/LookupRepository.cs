using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Mentora.Application.Interfaces;
using Mentora.Application.Interfaces.Repositories;
using Mentora.Domain.Entities;
using DomainEntity = Mentora.Domain.Entities.Domain;
using Mentora.Persistence;
namespace Mentora.Persistence.Repositories;


public class LookupRepository : ILookupRepository
{
    private readonly ApplicationDbContext _context;

    public LookupRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<DomainEntity>> GetDomainsAsync()
    {
        return await _context.Domains.ToListAsync();
    }

    public async Task<List<SubDomain>> GetSubDomainsByDomainIdAsync(int domainId)
    {
        return await _context.SubDomains
            .Where(s => s.DomainId == domainId)
            .ToListAsync();
    }

    public async Task<List<Technology>> GetTechnologiesBySubDomainIdAsync(int subDomainId)
    {
        return await _context.Technologies
            .Include(t => t.SubDomain)
            .Where(t => t.SubDomainId == subDomainId)
            .ToListAsync();
    }

    public async Task<List<CareerGoal>> GetCareerGoalsAsync()
    {
        return await _context.CareerGoals.ToListAsync();
    }

    public async Task<List<LearningStyle>> GetLearningStylesAsync()
    {
        return await _context.LearningStyles.ToListAsync();
    }

    public async Task<List<Country>> GetCountriesAsync()
    {
        return await _context.Countries.ToListAsync();
    }
}