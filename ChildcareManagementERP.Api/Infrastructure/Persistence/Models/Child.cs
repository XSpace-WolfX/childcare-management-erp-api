using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

[Table("Child", Schema = "Childcare")]
[Index("LastName", "FirstName", "BirthDate", Name = "IX_Child_NameBirth")]
public partial class Child
{
    [Key]
    public int Id { get; set; }

    [StringLength(1)]
    public string Gender { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    public DateOnly BirthDate { get; set; }

    [StringLength(100)]
    public string BirthCity { get; set; } = null!;

    public bool HasSiblings { get; set; }

    [StringLength(15)]
    public string? Phone { get; set; }

    [StringLength(256)]
    public string? Email { get; set; }

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [InverseProperty("Child")]
    public virtual ICollection<AdditionalData> AdditionalData { get; set; } = new List<AdditionalData>();

    [InverseProperty("Child")]
    public virtual ICollection<AuthorizedPersonChild> AuthorizedPersonChildren { get; set; } = new List<AuthorizedPersonChild>();

    [InverseProperty("Child")]
    public virtual ICollection<GuardianChild> GuardianChildren { get; set; } = new List<GuardianChild>();
}
