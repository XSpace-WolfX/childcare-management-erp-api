namespace ChildcareManagementERP.Api.Dtos.V1
{
    public class ChildDto
    {
        public int Id { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool? HasSiblings { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? BirthCity { get; set; }
    }

    public class ChildWithGuardiansDto
    {
        public int Id { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool? HasSiblings { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? BirthCity { get; set; }

        public List<GuardianDto> Guardians { get; set; } = new();
    }

    public class ChildWithAuthorizedPeopleDto
    {
        public int Id { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool? HasSiblings { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? BirthCity { get; set; }

        public List<AuthorizedPersonDto> AuthorizedPeople { get; set; } = new();
    }

    public class ChildWithAdditionalDatasDto
    {
        public int Id { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool? HasSiblings { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? BirthCity { get; set; }

        public List<AdditionalDataDto> AdditionalDatas { get; set; } = new();
    }

    public class CreateChildDto
    {
        public string Gender { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool? HasSiblings { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? BirthCity { get; set; }
    }

    public class UpdateChildDto
    {
        public int Id { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateOnly? BirthDate { get; set; }
        public bool? HasSiblings { get; set; }
        public string? Email { get; set; }
        public string? Phone { get; set; }
        public string? BirthCity { get; set; }
    }
}