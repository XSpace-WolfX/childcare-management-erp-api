using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

[Table("PersonalSituation", Schema = "Childcare")]
[Index("GuardianId", Name = "IX_PersonalSituation_GuardianId")]
public partial class PersonalSituation
{
    [Key]
    public int Id { get; set; }

    public int GuardianId { get; set; }

    [StringLength(100)]
    public string? MaritalStatus { get; set; }

    [StringLength(100)]
    public string? Sector { get; set; }

    [StringLength(100)]
    public string? Area { get; set; }

    [StringLength(100)]
    public string? Regime { get; set; }

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [ForeignKey("GuardianId")]
    [InverseProperty("PersonalSituations")]
    public virtual Guardian Guardian { get; set; } = null!;
}
