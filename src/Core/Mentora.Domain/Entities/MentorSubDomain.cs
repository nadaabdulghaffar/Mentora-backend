namespace Mentora.Domain.Entities;

public class MentorSubDomain
{
    public Guid MentorId { get; set; }
    public int SubDomainId { get; set; }

    public MentorProfile MentorProfile { get; set; } = null!;
    public SubDomain SubDomain { get; set; } = null!;
}