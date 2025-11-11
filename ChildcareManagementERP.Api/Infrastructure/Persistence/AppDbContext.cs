using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildcareManagementERP.Api.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdditionalDatum> AdditionalData { get; set; }

    public virtual DbSet<AuthorizedPerson> AuthorizedPeople { get; set; }

    public virtual DbSet<AuthorizedPersonChild> AuthorizedPersonChildren { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<FinancialInformation> FinancialInformations { get; set; }

    public virtual DbSet<Guardian> Guardians { get; set; }

    public virtual DbSet<GuardianChild> GuardianChildren { get; set; }

    public virtual DbSet<PersonalSituation> PersonalSituations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdditionalDatum>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Addition__3214EC07835450DE");

            entity.ToTable("AdditionalData", "Childcare", tb => tb.HasTrigger("TR_Childcare_AdditionalData_SetUpdatedUtc"));

            entity.HasIndex(e => new { e.ChildId, e.ParamName }, "UQ_AdditionalData_Child_ParamName").IsUnique();

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.ParamName).HasMaxLength(100);
            entity.Property(e => e.ParamType).HasMaxLength(15);
            entity.Property(e => e.ParamValue).HasMaxLength(256);
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Child).WithMany(p => p.AdditionalData)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK_AdditionalData_Child");
        });

        modelBuilder.Entity<AuthorizedPerson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authoriz__3214EC07E3CB16FE");

            entity.ToTable("AuthorizedPerson", "Childcare", tb => tb.HasTrigger("TR_Childcare_AuthorizedPerson_SetUpdatedUtc"));

            entity.HasIndex(e => e.Phone, "UQ_AuthorizedPerson_Phone").IsUnique();

            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<AuthorizedPersonChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authoriz__3214EC07E46551F9");

            entity.ToTable("AuthorizedPersonChild", "Childcare", tb => tb.HasTrigger("TR_Childcare_AuthorizedPersonChild_SetUpdatedUtc"));

            entity.HasIndex(e => e.AuthorizedPersonId, "IX_AuthorizedPersonChild_AuthorizedPersonId");

            entity.HasIndex(e => e.ChildId, "IX_AuthorizedPersonChild_ChildId");

            entity.HasIndex(e => new { e.AuthorizedPersonId, e.ChildId }, "UQ_AuthorizedPersonChild").IsUnique();

            entity.Property(e => e.Comment).HasMaxLength(1000);
            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Relationship).HasMaxLength(50);
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.AuthorizedPerson).WithMany(p => p.AuthorizedPersonChildren)
                .HasForeignKey(d => d.AuthorizedPersonId)
                .HasConstraintName("FK_AuthorizedPersonChild_AuthorizedPerson");

            entity.HasOne(d => d.Child).WithMany(p => p.AuthorizedPersonChildren)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK_AuthorizedPersonChild_Child");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Child__3214EC07D262C2EF");

            entity.ToTable("Child", "Childcare", tb => tb.HasTrigger("TR_Childcare_Child_SetUpdatedUtc"));

            entity.HasIndex(e => new { e.LastName, e.FirstName, e.BirthDate }, "IX_Child_NameBirth");

            entity.HasIndex(e => e.Email, "UQ_Child_Email")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL)");

            entity.HasIndex(e => e.Phone, "UQ_Child_Phone")
                .IsUnique()
                .HasFilter("([Phone] IS NOT NULL)");

            entity.Property(e => e.BirthCity).HasMaxLength(100);
            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.Gender).HasMaxLength(1);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<FinancialInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Financia__3214EC07074C33C6");

            entity.ToTable("FinancialInformation", "Childcare", tb => tb.HasTrigger("TR_Childcare_FinancialInformation_SetUpdatedUtc"));

            entity.HasIndex(e => e.GuardianId, "UX_FinancialInformation_GuardianId").IsUnique();

            entity.Property(e => e.AnnualIncome).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Model).HasMaxLength(100);
            entity.Property(e => e.MonthlyIncome).HasColumnType("decimal(10, 2)");
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Guardian).WithOne(p => p.FinancialInformation)
                .HasForeignKey<FinancialInformation>(d => d.GuardianId)
                .HasConstraintName("FK_FinancialInformation_Guardian");
        });

        modelBuilder.Entity<Guardian>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Guardian__3214EC0734D7B352");

            entity.ToTable("Guardian", "Childcare", tb => tb.HasTrigger("TR_Childcare_Guardian_SetUpdatedUtc"));

            entity.HasIndex(e => e.BeneficiaryNumber, "UQ_Guardian_BeneficiaryNumber")
                .IsUnique()
                .HasFilter("([BeneficiaryNumber] IS NOT NULL)");

            entity.HasIndex(e => e.Phone, "UQ_Guardian_Phone").IsUnique();

            entity.HasIndex(e => e.Phone2, "UQ_Guardian_Phone2")
                .IsUnique()
                .HasFilter("([Phone2] IS NOT NULL)");

            entity.Property(e => e.Address).HasMaxLength(256);
            entity.Property(e => e.BeneficiaryNumber).HasMaxLength(50);
            entity.Property(e => e.BirthName).HasMaxLength(100);
            entity.Property(e => e.City).HasMaxLength(100);
            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Email).HasMaxLength(256);
            entity.Property(e => e.FirstName).HasMaxLength(50);
            entity.Property(e => e.LastName).HasMaxLength(100);
            entity.Property(e => e.Phone).HasMaxLength(15);
            entity.Property(e => e.Phone2).HasMaxLength(15);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.Title).HasMaxLength(10);
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<GuardianChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Guardian__3214EC07AF15B2CB");

            entity.ToTable("GuardianChild", "Childcare", tb => tb.HasTrigger("TR_Childcare_GuardianChild_SetUpdatedUtc"));

            entity.HasIndex(e => e.ChildId, "IX_GuardianChild_ChildId");

            entity.HasIndex(e => e.GuardianId, "IX_GuardianChild_GuardianId");

            entity.HasIndex(e => new { e.GuardianId, e.ChildId }, "UQ_GuardianChild").IsUnique();

            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.Relationship).HasMaxLength(50);
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Child).WithMany(p => p.GuardianChildren)
                .HasForeignKey(d => d.ChildId)
                .HasConstraintName("FK_GuardianChild_Child");

            entity.HasOne(d => d.Guardian).WithMany(p => p.GuardianChildren)
                .HasForeignKey(d => d.GuardianId)
                .HasConstraintName("FK_GuardianChild_Guardian");
        });

        modelBuilder.Entity<PersonalSituation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personal__3214EC07961856EF");

            entity.ToTable("PersonalSituation", "Childcare", tb => tb.HasTrigger("TR_Childcare_PersonalSituation_SetUpdatedUtc"));

            entity.HasIndex(e => e.GuardianId, "UX_PersonalSituation_GuardianId").IsUnique();

            entity.Property(e => e.Area).HasMaxLength(100);
            entity.Property(e => e.CreatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.MaritalStatus).HasMaxLength(100);
            entity.Property(e => e.Regime).HasMaxLength(100);
            entity.Property(e => e.Sector).HasMaxLength(100);
            entity.Property(e => e.UpdatedUtc)
                .HasPrecision(2)
                .HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Guardian).WithOne(p => p.PersonalSituation)
                .HasForeignKey<PersonalSituation>(d => d.GuardianId)
                .HasConstraintName("FK_PersonalSituation_Guardian");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
