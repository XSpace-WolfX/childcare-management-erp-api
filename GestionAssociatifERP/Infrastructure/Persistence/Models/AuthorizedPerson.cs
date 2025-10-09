using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace GestionAssociatifERP.Infrastructure.Persistence.Models;

[Table("AuthorizedPerson", Schema = "Childcare")]
[Index("Phone", Name = "UQ_AuthorizedPerson_Phone", IsUnique = true)]
public partial class AuthorizedPerson
{
    [Key]
    public int Id { get; set; }

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(15)]
    public string Phone { get; set; } = null!;

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [InverseProperty("AuthorizedPerson")]
    public virtual ICollection<AuthorizedPersonChild> AuthorizedPersonChildren { get; set; } = new List<AuthorizedPersonChild>();
}
