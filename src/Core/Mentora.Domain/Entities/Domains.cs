public class Domains
{
    public int DomainId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<SubDomain> SubDomains { get; set; } = new List<SubDomain>();
    public ICollection<MenteeProfile> MenteeProfiles { get; set; } = new List<MenteeProfile>();
    public ICollection<MentorProfile> MentorProfiles { get; set; } = new List<MentorProfile>();
}
