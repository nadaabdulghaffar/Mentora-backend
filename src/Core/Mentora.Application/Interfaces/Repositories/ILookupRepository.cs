using System;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;



public interface ILookupRepository
{
    Task<List<Domains>> GetDomainsAsync();
    Task<List<SubDomain>> GetSubDomainsByDomainIdAsync(int domainId);
    Task<List<Technology>> GetTechnologiesBySubDomainIdAsync(int subDomainId);
    Task<List<CareerGoal>> GetCareerGoalsAsync();
    Task<List<LearningStyle>> GetLearningStylesAsync();
    Task<List<Country>> GetCountriesAsync();
}