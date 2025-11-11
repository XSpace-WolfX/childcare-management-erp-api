namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

public partial class Guardian
{
    public int Id { get; set; }

    public string Title { get; set; } = null!;

    public string LastName { get; set; } = null!;

    public string? BirthName { get; set; }

    public string FirstName { get; set; } = null!;

    public string Address { get; set; } = null!;

    public string PostalCode { get; set; } = null!;

    public string City { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string Phone { get; set; } = null!;

    public string? Phone2 { get; set; }

    public string? BeneficiaryNumber { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public virtual FinancialInformation? FinancialInformation { get; set; }

    public virtual ICollection<GuardianChild> GuardianChildren { get; set; } = new List<GuardianChild>();

    public virtual PersonalSituation? PersonalSituation { get; set; }
}
