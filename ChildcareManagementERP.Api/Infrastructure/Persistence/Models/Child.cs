namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

public partial class Child
{
    public int Id { get; set; }

    public string Gender { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    public string BirthCity { get; set; } = null!;

    public bool HasSiblings { get; set; }

    public string? Phone { get; set; }

    public string? Email { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public virtual ICollection<AdditionalDatum> AdditionalData { get; set; } = new List<AdditionalDatum>();

    public virtual ICollection<AuthorizedPersonChild> AuthorizedPersonChildren { get; set; } = new List<AuthorizedPersonChild>();

    public virtual ICollection<GuardianChild> GuardianChildren { get; set; } = new List<GuardianChild>();
}
