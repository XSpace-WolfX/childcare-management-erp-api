namespace GestionAssociatifERP.Dtos.V1
{
    public class AdditionalDataDto
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public string? ParamName { get; set; }
        public string? ParamValue { get; set; }
        public string? ParamType { get; set; }
        public string? Comment { get; set; }
    }

    public class CreateAdditionalDataDto
    {
        public int ChildId { get; set; }
        public string? ParamName { get; set; }
        public string? ParamValue { get; set; }
        public string? ParamType { get; set; }
        public string? Comment { get; set; }
    }

    public class UpdateAdditionalDataDto
    {
        public int Id { get; set; }
        public int ChildId { get; set; }
        public string? ParamName { get; set; }
        public string? ParamValue { get; set; }
        public string? ParamType { get; set; }
        public string? Comment { get; set; }
    }
}