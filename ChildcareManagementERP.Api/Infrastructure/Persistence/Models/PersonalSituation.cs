namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

public partial class PersonalSituation
{
    public int Id { get; set; }

    public int GuardianId { get; set; }

    public string? MaritalStatus { get; set; }

    public string? Sector { get; set; }

    public string? Area { get; set; }

    public string? Regime { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public virtual Guardian Guardian { get; set; } = null!;
}
