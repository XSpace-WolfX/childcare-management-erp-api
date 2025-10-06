using System.Text.Json.Serialization;

namespace GestionAssociatifERP.Dtos.V1
{
    public class EnfantDto
    {
        public int Id { get; set; }
        public string Civilite { get; set; } = string.Empty;
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateOnly? DateNaissance { get; set; }
        public bool? PresenceFratrie { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? VilleNaissance { get; set; }
    }

    public class EnfantWithResponsablesDto
    {
        public int Id { get; set; }
        public string Civilite { get; set; } = string.Empty;
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateOnly? DateNaissance { get; set; }
        public bool? PresenceFratrie { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? VilleNaissance { get; set; }

        public List<ResponsableDto> Responsables { get; set; } = new();
    }

    public class EnfantWithPersonnesAutoriseesDto
    {
        public int Id { get; set; }
        public string Civilite { get; set; } = string.Empty;
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateOnly? DateNaissance { get; set; }
        public bool? PresenceFratrie { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? VilleNaissance { get; set; }

        public List<PersonneAutoriseeDto> PersonnesAutorisees { get; set; } = new();
    }

    public class EnfantWithDonneesSupplementairesDto
    {
        public int Id { get; set; }
        public string Civilite { get; set; } = string.Empty;
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateOnly? DateNaissance { get; set; }
        public bool? PresenceFratrie { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? VilleNaissance { get; set; }

        public List<DonneeSupplementaireDto> DonneeSupplementaires { get; set; } = new();
    }

    public class CreateEnfantDto
    {
        [JsonPropertyName("gender")]
        public string Civilite { get; set; } = string.Empty;
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateOnly? DateNaissance { get; set; }
        public bool? PresenceFratrie { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? VilleNaissance { get; set; }
    }

    public class UpdateEnfantDto
    {
        public int Id { get; set; }
        public string Civilite { get; set; } = string.Empty;
        public string? Nom { get; set; }
        public string? Prenom { get; set; }
        public DateOnly? DateNaissance { get; set; }
        public bool? PresenceFratrie { get; set; }
        public string? Email { get; set; }
        public string? Telephone { get; set; }
        public string? VilleNaissance { get; set; }
    }
}
