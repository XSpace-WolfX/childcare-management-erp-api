using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAssociatifERP.Infrastructure.Persistence.Models;

[Table("AdditionalData", Schema = "Childcare")]
[Index("ChildId", "ParamName", Name = "UQ_AdditionalData_Child_ParamName", IsUnique = true)]
public partial class AdditionalData
{
    [Key]
    public int Id { get; set; }

    public int ChildId { get; set; }

    [StringLength(100)]
    public string ParamName { get; set; } = null!;

    [StringLength(256)]
    public string ParamValue { get; set; } = null!;

    [StringLength(15)]
    public string ParamType { get; set; } = null!;

    [StringLength(1000)]
    public string? Comment { get; set; }

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [ForeignKey("ChildId")]
    [InverseProperty("AdditionalData")]
    public virtual Child Child { get; set; } = null!;
}
