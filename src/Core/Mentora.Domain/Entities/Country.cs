namespace Mentora.Domain.Entities;

public class Country
{
    public string CountryCode { get; set; } = null!;
    public string CountryName { get; set; } = null!;

    public ICollection<MenteeProfile> MenteeProfiles { get; set; } = new List<MenteeProfile>();
    public ICollection<MentorProfile> MentorProfiles { get; set; } = new List<MentorProfile>();
}