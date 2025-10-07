using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAssociatifERP.Infrastructure.Persistence.Models;

[Table("FinancialInformation", Schema = "Childcare")]
[Index("GuardianId", Name = "IX_FinancialInformation_GuardianId")]
public partial class FinancialInformation
{
    [Key]
    public int Id { get; set; }

    public int GuardianId { get; set; }

    public int? FamilyQuotient { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? MonthlyIncome { get; set; }

    [Column(TypeName = "decimal(10, 2)")]
    public decimal? AnnualIncome { get; set; }

    [StringLength(100)]
    public string? Model { get; set; }

    public DateOnly? StartDate { get; set; }

    public DateOnly? EndDate { get; set; }

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [ForeignKey("GuardianId")]
    [InverseProperty("FinancialInformations")]
    public virtual Guardian Guardian { get; set; } = null!;
}
