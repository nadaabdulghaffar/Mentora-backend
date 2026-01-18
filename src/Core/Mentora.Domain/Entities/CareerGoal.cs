namespace Mentora.Domain.Entities;

public class CareerGoal
{
    public int CareerGoalId { get; set; }
    public string Name { get; set; } = null!;

    public ICollection<MenteeProfile> MenteeProfiles { get; set; } = new List<MenteeProfile>();
}