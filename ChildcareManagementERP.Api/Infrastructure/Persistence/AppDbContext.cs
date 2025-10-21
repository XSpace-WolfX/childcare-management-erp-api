using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using Microsoft.EntityFrameworkCore;

namespace ChildcareManagementERP.Api.Infrastructure.Persistence;

public partial class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AdditionalData> AdditionalData { get; set; }

    public virtual DbSet<AuthorizedPerson> AuthorizedPeople { get; set; }

    public virtual DbSet<AuthorizedPersonChild> AuthorizedPersonChildren { get; set; }

    public virtual DbSet<Child> Children { get; set; }

    public virtual DbSet<FinancialInformation> FinancialInformations { get; set; }

    public virtual DbSet<Guardian> Guardians { get; set; }

    public virtual DbSet<GuardianChild> GuardianChildren { get; set; }

    public virtual DbSet<PersonalSituation> PersonalSituations { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AdditionalData>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Addition__3214EC07835450DE");

            entity.ToTable("AdditionalData", "Childcare", tb => tb.HasTrigger("TR_Childcare_AdditionalData_SetUpdatedUtc"));

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Child).WithMany(p => p.AdditionalData).HasConstraintName("FK_AdditionalData_Child");
        });

        modelBuilder.Entity<AuthorizedPerson>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authoriz__3214EC07E3CB16FE");

            entity.ToTable("AuthorizedPerson", "Childcare", tb => tb.HasTrigger("TR_Childcare_AuthorizedPerson_SetUpdatedUtc"));

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<AuthorizedPersonChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Authoriz__3214EC07E46551F9");

            entity.ToTable("AuthorizedPersonChild", "Childcare", tb => tb.HasTrigger("TR_Childcare_AuthorizedPersonChild_SetUpdatedUtc"));

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.AuthorizedPerson).WithMany(p => p.AuthorizedPersonChildren).HasConstraintName("FK_AuthorizedPersonChild_AuthorizedPerson");

            entity.HasOne(d => d.Child).WithMany(p => p.AuthorizedPersonChildren).HasConstraintName("FK_AuthorizedPersonChild_Child");
        });

        modelBuilder.Entity<Child>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Child__3214EC07D262C2EF");

            entity.ToTable("Child", "Childcare", tb => tb.HasTrigger("TR_Childcare_Child_SetUpdatedUtc"));

            entity.HasIndex(e => e.Email, "UQ_Child_Email")
                .IsUnique()
                .HasFilter("([Email] IS NOT NULL)");

            entity.HasIndex(e => e.Phone, "UQ_Child_Phone")
                .IsUnique()
                .HasFilter("([Phone] IS NOT NULL)");

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<FinancialInformation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Financia__3214EC07074C33C6");

            entity.ToTable("FinancialInformation", "Childcare", tb => tb.HasTrigger("TR_Childcare_FinancialInformation_SetUpdatedUtc"));

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Guardian).WithMany(p => p.FinancialInformations).HasConstraintName("FK_FinancialInformation_Guardian");
        });

        modelBuilder.Entity<Guardian>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Guardian__3214EC0734D7B352");

            entity.ToTable("Guardian", "Childcare", tb => tb.HasTrigger("TR_Childcare_Guardian_SetUpdatedUtc"));

            entity.HasIndex(e => e.BeneficiaryNumber, "UQ_Guardian_BeneficiaryNumber")
                .IsUnique()
                .HasFilter("([BeneficiaryNumber] IS NOT NULL)");

            entity.HasIndex(e => e.Phone2, "UQ_Guardian_Phone2")
                .IsUnique()
                .HasFilter("([Phone2] IS NOT NULL)");

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");
        });

        modelBuilder.Entity<GuardianChild>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Guardian__3214EC07AF15B2CB");

            entity.ToTable("GuardianChild", "Childcare", tb => tb.HasTrigger("TR_Childcare_GuardianChild_SetUpdatedUtc"));

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Child).WithMany(p => p.GuardianChildren).HasConstraintName("FK_GuardianChild_Child");

            entity.HasOne(d => d.Guardian).WithMany(p => p.GuardianChildren).HasConstraintName("FK_GuardianChild_Guardian");
        });

        modelBuilder.Entity<PersonalSituation>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__Personal__3214EC07961856EF");

            entity.ToTable("PersonalSituation", "Childcare", tb => tb.HasTrigger("TR_Childcare_PersonalSituation_SetUpdatedUtc"));

            entity.Property(e => e.CreatedUtc).HasDefaultValueSql("(sysutcdatetime())");
            entity.Property(e => e.UpdatedUtc).HasDefaultValueSql("(sysutcdatetime())");

            entity.HasOne(d => d.Guardian).WithMany(p => p.PersonalSituations).HasConstraintName("FK_PersonalSituation_Guardian");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
