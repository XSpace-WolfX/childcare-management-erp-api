using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ChildcareManagementERP.Api.Infrastructure.Persistence.Models;

[Table("Guardian", Schema = "Childcare")]
[Index("Phone", Name = "UQ_Guardian_Phone", IsUnique = true)]
public partial class Guardian
{
    [Key]
    public int Id { get; set; }

    [StringLength(10)]
    public string Title { get; set; } = null!;

    [StringLength(100)]
    public string LastName { get; set; } = null!;

    [StringLength(100)]
    public string? BirthName { get; set; }

    [StringLength(50)]
    public string FirstName { get; set; } = null!;

    [StringLength(256)]
    public string Address { get; set; } = null!;

    [StringLength(20)]
    public string PostalCode { get; set; } = null!;

    [StringLength(100)]
    public string City { get; set; } = null!;

    [StringLength(256)]
    public string Email { get; set; } = null!;

    [StringLength(15)]
    public string Phone { get; set; } = null!;

    [StringLength(15)]
    public string? Phone2 { get; set; }

    [StringLength(50)]
    public string? BeneficiaryNumber { get; set; }

    [Precision(2)]
    public DateTime CreatedUtc { get; set; }

    [Precision(2)]
    public DateTime UpdatedUtc { get; set; }

    [InverseProperty("Guardian")]
    public virtual ICollection<FinancialInformation> FinancialInformations { get; set; } = new List<FinancialInformation>();

    [InverseProperty("Guardian")]
    public virtual ICollection<GuardianChild> GuardianChildren { get; set; } = new List<GuardianChild>();

    [InverseProperty("Guardian")]
    public virtual ICollection<PersonalSituation> PersonalSituations { get; set; } = new List<PersonalSituation>();
}
