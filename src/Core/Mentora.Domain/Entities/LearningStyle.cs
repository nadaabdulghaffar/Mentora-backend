public class LearningStyle
{
    public int LearningStyleId { get; set; }
    public string Name { get; set; } = null!;
    public string? Description { get; set; }

    public ICollection<MenteeProfile> MenteeProfiles { get; set; } = new List<MenteeProfile>();
}