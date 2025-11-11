namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

public partial class FinancialInformation
{
    public int Id { get; set; }

    public int GuardianId { get; set; }

    public int? FamilyQuotient { get; set; }

    public decimal? MonthlyIncome { get; set; }

    public decimal? AnnualIncome { get; set; }

    public string? Model { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    public DateTime CreatedUtc { get; set; }

    public DateTime UpdatedUtc { get; set; }

    public virtual Guardian Guardian { get; set; } = null!;
}
