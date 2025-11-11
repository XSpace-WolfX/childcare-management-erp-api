namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

public partial class AuthorizedPerson
{
    public int Id { get; set; }

    public string LastName { get; set; } = null!;

    public string FirstName { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public virtual ICollection<AuthorizedPersonChild> AuthorizedPersonChildren { get; set; } = new List<AuthorizedPersonChild>();
}
