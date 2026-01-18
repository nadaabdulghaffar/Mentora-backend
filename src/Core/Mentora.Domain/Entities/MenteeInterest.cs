using Mentora.Domain.Enums;

namespace Mentora.Domain.Entities;

public class MenteeInterest
{
    public Guid UserId { get; set; }
    public int TechnologyId { get; set; }
    public ExperienceLevel ExperienceLevel { get; set; }

    public MenteeProfile MenteeProfile { get; set; } = null!;
    public Technology Technology { get; set; } = null!;
}