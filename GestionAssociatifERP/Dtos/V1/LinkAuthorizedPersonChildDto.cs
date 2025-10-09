namespace GestionAssociatifERP.Dtos.V1
{
    public class LinkAuthorizedPersonChildDto
    {
        public int AuthorizedPersonId { get; set; }
        public int ChildId { get; set; }
        public string? Relationship { get; set; }
        public bool? EmergencyContact { get; set; }
        public string? Comment { get; set; }
    }

    public class CreateLinkAuthorizedPersonChildDto
    {
        public int AuthorizedPersonId { get; set; }
        public int ChildId { get; set; }
        public string? Relationship { get; set; }
        public bool? EmergencyContact { get; set; }
        public string? Comment { get; set; }
    }

    public class UpdateLinkAuthorizedPersonChildDto
    {
        public int AuthorizedPersonId { get; set; }
        public int ChildId { get; set; }
        public string? Relationship { get; set; }
        public bool? EmergencyContact { get; set; }
        public string? Comment { get; set; }
    }
}