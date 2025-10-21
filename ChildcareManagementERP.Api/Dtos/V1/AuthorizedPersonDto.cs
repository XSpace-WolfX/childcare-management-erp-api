namespace ChildcareManagementERP.Api.Dtos.V1
{
    public class AuthorizedPersonDto
    {
        public int Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Phone { get; set; }
    }

    public class AuthorizedPersonWithChildrenDto
    {
        public int Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Phone { get; set; }

        public List<ChildDto> Children { get; set; } = new();
    }

    public class CreateAuthorizedPersonDto
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Phone { get; set; }
    }

    public class UpdateAuthorizedPersonDto
    {
        public int Id { get; set; }
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public string? Phone { get; set; }
    }
}