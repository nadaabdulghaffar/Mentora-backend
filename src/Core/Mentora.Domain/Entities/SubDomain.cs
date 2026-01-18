namespace Mentora.Domain.Entities;

public class SubDomain
{
    public int SubDomainId { get; set; }
    public int DomainId { get; set; }
    public string Name { get; set; } = null!;

    public Domain Domain { get; set; } = null!;
    public ICollection<Technology> Technologies { get; set; } = new List<Technology>();
}
