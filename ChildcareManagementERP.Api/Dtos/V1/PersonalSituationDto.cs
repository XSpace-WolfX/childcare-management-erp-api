namespace ChildcareManagementERP.Api.Dtos.V1
{
    public class PersonalSituationDto
    {
        public int Id { get; set; }
        public int GuardianId { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Sector { get; set; }
        public string? Area { get; set; }
        public string? Regime { get; set; }
    }

    public class CreatePersonalSituationDto
    {
        public int GuardianId { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Sector { get; set; }
        public string? Area { get; set; }
        public string? Regime { get; set; }
    }

    public class UpdatePersonalSituationDto
    {
        public int Id { get; set; }
        public int GuardianId { get; set; }
        public string? MaritalStatus { get; set; }
        public string? Sector { get; set; }
        public string? Area { get; set; }
        public string? Regime { get; set; }
    }
}