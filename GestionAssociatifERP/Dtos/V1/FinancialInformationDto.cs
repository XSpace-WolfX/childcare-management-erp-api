namespace GestionAssociatifERP.Dtos.V1
{
    public class FinancialInformationDto
    {
        public int Id { get; set; }
        public int GuardianId { get; set; }
        public int? FamilyQuotient { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public decimal? AnnualIncome { get; set; }
        public string? Model { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class CreateFinancialInformationDto
    {
        public int GuardianId { get; set; }
        public int? FamilyQuotient { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public decimal? AnnualIncome { get; set; }
        public string? Model { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }

    public class UpdateFinancialInformationDto
    {
        public int Id { get; set; }
        public int GuardianId { get; set; }
        public int? FamilyQuotient { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public decimal? AnnualIncome { get; set; }
        public string? Model { get; set; }
        public DateOnly? StartDate { get; set; }
        public DateOnly? EndDate { get; set; }
    }
}