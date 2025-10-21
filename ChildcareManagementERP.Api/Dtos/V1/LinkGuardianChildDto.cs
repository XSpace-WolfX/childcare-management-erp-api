namespace ChildcareManagementERP.Api.Dtos.V1
{
    public class LinkGuardianChildDto
    {
        public int GuardianId { get; set; }
        public int ChildId { get; set; }
        public string? Relationship { get; set; }
    }

    public class CreateLinkGuardianChildDto
    {
        public int GuardianId { get; set; }
        public int ChildId { get; set; }
        public string? Relationship { get; set; }
    }

    public class UpdateLinkGuardianChildDto
    {
        public int GuardianId { get; set; }
        public int ChildId { get; set; }
        public string? Relationship { get; set; }
    }
}