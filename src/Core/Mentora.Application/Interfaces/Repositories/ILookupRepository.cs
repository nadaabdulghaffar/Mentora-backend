using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Mentora.Application.DTOs.Auth;
using Mentora.Application.DTOs.Common;
using Mentora.Domain.Entities;
using DomainEntity = Mentora.Domain.Entities.Domain;

public interface ILookupRepository
{
    Task<List<DomainEntity>> GetDomainsAsync();
    Task<List<SubDomain>> GetSubDomainsByDomainIdAsync(int domainId);
    Task<List<Technology>> GetTechnologiesBySubDomainIdAsync(int subDomainId);
    Task<List<CareerGoal>> GetCareerGoalsAsync();
    Task<List<LearningStyle>> GetLearningStylesAsync();
    Task<List<Country>> GetCountriesAsync();
}