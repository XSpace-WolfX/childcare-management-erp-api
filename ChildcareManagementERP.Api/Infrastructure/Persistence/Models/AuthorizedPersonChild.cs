using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

[Table("AuthorizedPersonChild", Schema = "Childcare")]
[Index("AuthorizedPersonId", Name = "IX_AuthorizedPersonChild_AuthorizedPersonId")]
[Index("ChildId", Name = "IX_AuthorizedPersonChild_ChildId")]
[Index("AuthorizedPersonId", "ChildId", Name = "UQ_AuthorizedPersonChild", IsUnique = true)]
public partial class AuthorizedPersonChild
{
    [Key]
    public int Id { get; set; }

    public int AuthorizedPersonId { get; set; }

    public int ChildId { get; set; }

    [StringLength(50)]
    public string Relationship { get; set; } = null!;

    public bool EmergencyContact { get; set; }

    [StringLength(1000)]
    public string? Comment { get; set; }

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [ForeignKey("AuthorizedPersonId")]
    [InverseProperty("AuthorizedPersonChildren")]
    public virtual AuthorizedPerson AuthorizedPerson { get; set; } = null!;

    [ForeignKey("ChildId")]
    [InverseProperty("AuthorizedPersonChildren")]
    public virtual Child Child { get; set; } = null!;
}
