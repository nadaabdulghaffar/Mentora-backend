public class Technology
{
    public int TechnologyId { get; set; }
    public int SubDomainId { get; set; }
    public string Name { get; set; } = null!;

    public SubDomain SubDomain { get; set; } = null!;
    public ICollection<MenteeInterest> MenteeInterests { get; set; } = new List<MenteeInterest>();
    public ICollection<MentorExpertise> MentorExpertises { get; set; } = new List<MentorExpertise>();
}