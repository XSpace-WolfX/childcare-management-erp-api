namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

public partial class AdditionalDatum
{
    public int Id { get; set; }

    public int ChildId { get; set; }

    public string ParamName { get; set; } = null!;

    public string ParamValue { get; set; } = null!;

    public string ParamType { get; set; } = null!;

    public string? Comment { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public virtual Child Child { get; set; } = null!;
}
