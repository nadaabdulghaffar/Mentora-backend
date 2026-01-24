namespace Mentora.Domain.Entities;

public class MenteeSubDomain
{
    public Guid UserId { get; set; }
    public int SubDomainId { get; set; }

    public MenteeProfile MenteeProfile { get; set; } = null!;
    public SubDomain SubDomain { get; set; } = null!;
}