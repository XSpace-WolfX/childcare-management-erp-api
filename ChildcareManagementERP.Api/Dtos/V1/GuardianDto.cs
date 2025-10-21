namespace ChildcareManagementERP.Api.Dtos.V1
{
    public class GuardianDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? BirthName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? BeneficiaryNumber { get; set; }
    }

    public class GuardianWithFinancialInformationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? BirthName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? BeneficiaryNumber { get; set; }

        public FinancialInformationDto? FinancialInformation { get; set; }
    }

    public class GuardianWithPersonalSituationDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? BirthName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? BeneficiaryNumber { get; set; }

        public PersonalSituationDto? PersonalSituation { get; set; }
    }

    public class GuardianWithChildrenDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? BirthName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? BeneficiaryNumber { get; set; }

        public List<ChildDto> Children { get; set; } = new();
    }

    public class CreateGuardianDto
    {
        public string Title { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? BirthName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? BeneficiaryNumber { get; set; }
    }

    public class UpdateGuardianDto
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? BirthName { get; set; }
        public string? FirstName { get; set; }
        public string? Email { get; set; }
        public string? Address { get; set; }
        public string? PostalCode { get; set; }
        public string? City { get; set; }
        public string? Phone { get; set; }
        public string? Phone2 { get; set; }
        public string? BeneficiaryNumber { get; set; }
    }
}