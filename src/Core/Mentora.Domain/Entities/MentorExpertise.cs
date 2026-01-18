using Mentora.Domain.Enums;

namespace Mentora.Domain.Entities;

public class MentorExpertise
{
    public Guid MentorId { get; set; }
    public int TechnologyId { get; set; }

    public MentorProfile MentorProfile { get; set; } = null!;
    public Technology Technology { get; set; } = null!;
}