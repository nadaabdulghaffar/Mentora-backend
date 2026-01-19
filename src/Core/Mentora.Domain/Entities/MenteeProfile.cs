using Mentora.Domain.Entities;

public class MenteeProfile
{
    public Guid UserId { get; set; }
    public int DomainId { get; set; }
    public ExperienceLevel CurrentLevel { get; set; }
    public EducationStatus EducationStatus { get; set; }
    public int? CareerGoalId { get; set; }
    public int? LearningStyleId { get; set; }
    public string? CountryCode { get; set; }
    public string? ProfilePictureUrl { get; set; }
    public string? Bio { get; set; }
    public bool IsEmailVerified { get; set; }

    public User User { get; set; } = null!;
    public Domains Domain { get; set; } = null!;
    public CareerGoal? CareerGoal { get; set; }
    public LearningStyle? LearningStyle { get; set; }
    public Country? Country { get; set; }
    public ICollection<MenteeInterest> MenteeInterests { get; set; } = new List<MenteeInterest>();
}
