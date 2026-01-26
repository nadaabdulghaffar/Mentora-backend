public class CompleteMentorProfileRequest
{
    public string RegistrationToken { get; set; } = null!;  // From previous step
    public int DomainId { get; set; }
    public int YearsOfExperience { get; set; }
    public string? LinkedInUrl { get; set; }
    public List<int> SubDomainIds { get; set; } = new();
    public List<int> TechnologyIds { get; set; } = new();
    public string? CvUrl { get; set; }
    public string? Bio { get; set; }
    public string? CountryCode { get; set; }
}