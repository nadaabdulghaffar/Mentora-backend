using System;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;

using MentorshipPlatform.Domain.Entities;

public interface ILookupRepository
{
    Task<List<Domain>> GetDomainsAsync();
    Task<List<SubDomain>> GetSubDomainsByDomainIdAsync(int domainId);
    Task<List<Technology>> GetTechnologiesBySubDomainIdAsync(int subDomainId);
    Task<List<CareerGoal>> GetCareerGoalsAsync();
    Task<List<LearningStyle>> GetLearningStylesAsync();
    Task<List<Country>> GetCountriesAsync();
}