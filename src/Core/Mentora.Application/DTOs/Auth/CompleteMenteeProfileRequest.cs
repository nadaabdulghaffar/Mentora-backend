namespace Mentora.Application.DTOs.Auth;

public class CompleteMenteeProfileRequest
{
    public string RegistrationToken { get; set; } = null!;  // From previous step
    public int DomainId { get; set; }
	public string EducationStatus { get; set; } = null!;
	public int? CareerGoalId { get; set; }
	public int? LearningStyleId { get; set; }
	public string ExperienceLevel { get; set; } = null!;
    public List<int> SubDomainIds { get; set; } = new();
    public List<int> TechnologyIds { get; set; } = new();
	public string? Bio { get; set; }
	public string? CountryCode { get; set; }
}