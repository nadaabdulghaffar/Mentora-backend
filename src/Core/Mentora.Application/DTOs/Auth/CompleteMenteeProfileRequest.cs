namespace Mentora.Application.DTOs.Auth;

public class CompleteMenteeProfileRequest
{
	public Guid UserId { get; set; }
	public int DomainId { get; set; }
	public string EducationStatus { get; set; } = null!;
	public int? CareerGoalId { get; set; }
	public int? LearningStyleId { get; set; }
	public string ExperienceLevel { get; set; } = null!;
	public List<int> TechnologyIds { get; set; } = new();
	public string? Bio { get; set; }
	public string? CountryCode { get; set; }
}