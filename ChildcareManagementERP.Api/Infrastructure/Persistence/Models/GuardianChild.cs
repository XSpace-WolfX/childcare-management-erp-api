using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

[Table("GuardianChild", Schema = "Childcare")]
[Index("ChildId", Name = "IX_GuardianChild_ChildId")]
[Index("GuardianId", Name = "IX_GuardianChild_GuardianId")]
[Index("GuardianId", "ChildId", Name = "UQ_GuardianChild", IsUnique = true)]
public partial class GuardianChild
{
    [Key]
    public int Id { get; set; }

    public int GuardianId { get; set; }

    public int ChildId { get; set; }

    [StringLength(50)]
    public string Relationship { get; set; } = null!;

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [ForeignKey("ChildId")]
    [InverseProperty("GuardianChildren")]
    public virtual Child Child { get; set; } = null!;

    [ForeignKey("GuardianId")]
    [InverseProperty("GuardianChildren")]
    public virtual Guardian Guardian { get; set; } = null!;
}
