namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

public partial class AuthorizedPersonChild
{
    public int Id { get; set; }

    public int AuthorizedPersonId { get; set; }

    public int ChildId { get; set; }

    public string Relationship { get; set; } = null!;

    public bool EmergencyContact { get; set; }

    public string? Comment { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public virtual AuthorizedPerson AuthorizedPerson { get; set; } = null!;

    public virtual Child Child { get; set; } = null!;
}
