using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Mentora.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LookupController : ControllerBase
{
    private readonly IUnitOfWork _unitOfWork;

    public LookupController(IUnitOfWork unitOfWork)
    {
        _unitOfWork = unitOfWork;
    }

    [HttpGet("domains")]
    public async Task<IActionResult> GetDomains()
    {
        var domains = await _unitOfWork.Lookups.GetDomainsAsync();
        return Ok(domains);
    }

    [HttpGet("domains/{domainId}/subdomains")]
    public async Task<IActionResult> GetSubDomains(int domainId)
    {
        var subDomains = await _unitOfWork.Lookups.GetSubDomainsByDomainIdAsync(domainId);
        return Ok(subDomains);
    }

    [HttpGet("subdomains/{subDomainId}/technologies")]
    public async Task<IActionResult> GetTechnologies(int subDomainId)
    {
        var technologies = await _unitOfWork.Lookups.GetTechnologiesBySubDomainIdAsync(subDomainId);
        return Ok(technologies);
    }

    [HttpGet("career-goals")]
    public async Task<IActionResult> GetCareerGoals()
    {
        var goals = await _unitOfWork.Lookups.GetCareerGoalsAsync();
        return Ok(goals);
    }

    [HttpGet("learning-styles")]
    public async Task<IActionResult> GetLearningStyles()
    {
        var styles = await _unitOfWork.Lookups.GetLearningStylesAsync();
        return Ok(styles);
    }

    [HttpGet("countries")]
    public async Task<IActionResult> GetCountries()
    {
        var countries = await _unitOfWork.Lookups.GetCountriesAsync();
        return Ok(countries);
    }

    [HttpGet("education-statuses")]
    public IActionResult GetEducationStatuses()
    {
        var statuses = Enum.GetValues<Domain.Enums.EducationStatus>()
            .Select(e => new { Value = (int)e, Name = e.ToString() });
        return Ok(statuses);
    }

    [HttpGet("experience-levels")]
    public IActionResult GetExperienceLevels()
    {
        var levels = Enum.GetValues<Domain.Enums.ExperienceLevel>()
            .Select(e => new { Value = (int)e, Name = e.ToString() });
        return Ok(levels);
    }
}