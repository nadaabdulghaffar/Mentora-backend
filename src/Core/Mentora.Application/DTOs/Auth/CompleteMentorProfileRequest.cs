public class CompleteMentorProfileRequest
{
    public Guid UserId { get; set; }
    public int DomainId { get; set; }
    public int YearsOfExperience { get; set; }
    public string? LinkedInUrl { get; set; }
    public List<int> TechnologyIds { get; set; } = new();
    public string? CvUrl { get; set; }
    public string? Bio { get; set; }
    public string? CountryCode { get; set; }
}