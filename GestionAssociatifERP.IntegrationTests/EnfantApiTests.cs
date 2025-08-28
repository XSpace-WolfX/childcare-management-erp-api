using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class EnfantApiTests
    {
        [Fact]
        public async Task GetAllEnfants_ShouldReturnEnfants_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/enfants";
            var dto = new CreateEnfantDto { Nom = "Alice" };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var enfants = await response.Content.ReadFromJsonAsync<List<EnfantDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldContain(e => e.Nom == "Alice");
        }

        [Fact]
        public async Task GetAllEnfants_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/enfants";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var enfants = await response.Content.ReadFromJsonAsync<List<EnfantDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantById_ShouldReturnEnfant_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateEnfantDto { Nom = "Alice", Civilite = "mme" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<EnfantDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var enfant = await response.Content.ReadFromJsonAsync<EnfantDto>();
            enfant.ShouldNotBeNull();
            enfant.Id.ShouldBe(created.Id);
            enfant.Nom.ShouldBe("Alice");
        }

        [Fact]
        public async Task GetEnfantById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithResponsables_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un enfant
            var enfantDto = new CreateEnfantDto { Nom = "Alice", Civilite = "mme" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<EnfantDto>();

            // 2. Créer un responsable
            var responsableDto = new CreateResponsableDto { Nom = "Dupont", Civilite = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            // 3. Lier responsable ↔ enfant
            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = createdEnfant!.Id,
                ResponsableId = createdResponsable!.Id,
                Affiliation = "Parent"
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{createdEnfant.Id}/with-responsables");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<EnfantWithResponsablesDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdEnfant.Id);
            result.Responsables.ShouldNotBeNull();
            result.Responsables.ShouldContain(r => r.Nom == "Dupont");
        }

        [Fact]
        public async Task GetWithResponsables_ShouldReturnEmptyList_WhenNoResponsables()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateEnfantDto { Nom = "Léo", Civilite = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<EnfantDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}/with-responsables");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<EnfantWithResponsablesDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.Responsables.ShouldNotBeNull();
            result.Responsables.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetWithResponsables_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999/with-responsables");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithPersonnesAutorisees_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un enfant
            var enfantDto = new CreateEnfantDto { Nom = "Léo", Civilite = "m" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<EnfantDto>();

            // 2. Créer une personne autorisée
            var personneDto = new CreatePersonneAutoriseeDto { Nom = "Martin", Prenom = "Matin" };
            var postPers = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneDto);
            postPers.EnsureSuccessStatusCode();
            var createdPers = await postPers.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();

            // 3. Lier les deux
            var linkDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = createdEnfant!.Id,
                PersonneAutoriseeId = createdPers!.Id
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{createdEnfant.Id}/with-personnes-autorisees");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<EnfantWithPersonnesAutoriseesDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdEnfant.Id);
            result.PersonnesAutorisees.ShouldNotBeNull();
            result.PersonnesAutorisees.ShouldContain(p => p.Nom == "Martin");
        }

        [Fact]
        public async Task GetWithPersonnesAutorisees_ShouldReturnEmptyList_WhenNoPersonnesAutorisees()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfantDto = new CreateEnfantDto { Nom = "Sasha", Civilite = "non_specifie" };
            var post = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<EnfantDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}/with-personnes-autorisees");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<EnfantWithPersonnesAutoriseesDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.PersonnesAutorisees.ShouldNotBeNull();
            result.PersonnesAutorisees.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetWithPersonnesAutorisees_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999/with-personnes-autorisees");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithDonneesSupplementaires_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Créer un enfant
            var enfantDto = new CreateEnfantDto { Nom = "Jules", Civilite = "m" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var created = await postEnfant.Content.ReadFromJsonAsync<EnfantDto>();

            // Créer une donnée supplémentaire liée à l’enfant
            var dsDto = new CreateDonneeSupplementaireDto
            {
                EnfantId = created!.Id,
                Parametre = "Allergie",
                Valeur = "Gluten",
                Type = "Texte",
                Commentaire = "Important"
            };

            var postDs = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", dsDto);
            postDs.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created.Id}/with-donnees-supplementaires");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<EnfantWithDonneesSupplementairesDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.DonneeSupplementaires.ShouldNotBeNull();
            result.DonneeSupplementaires.ShouldContain(d => d.Parametre == "Allergie");
        }

        [Fact]
        public async Task GetWithDonneesSupplementaires_ShouldReturnEmptyList_WhenNoDonneesSupplementaires()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfantDto = new CreateEnfantDto { Nom = "Sami", Civilite = "autre" };
            var post = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<EnfantDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}/with-donnees-supplementaires");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<EnfantWithDonneesSupplementairesDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.DonneeSupplementaires.ShouldNotBeNull();
            result.DonneeSupplementaires.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetWithDonneesSupplementaires_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999/with-donnees-supplementaires");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateEnfant_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new CreateEnfantDto
            {
                Civilite = "mme",
                Nom = "Camille",
                Prenom = "Dupont"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/enfants", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<EnfantDto>();
            created.ShouldNotBeNull();
            created.Nom.ShouldBe("Camille");
            created.Civilite.ShouldBe("mme");
        }

        [Fact]
        public async Task UpdateEnfant_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateEnfantDto { Nom = "Emma", Civilite = "mme" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<EnfantDto>();

            var updateDto = new UpdateEnfantDto
            {
                Id = created!.Id,
                Nom = "Emma Modifiée",
                Civilite = "mme"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/enfants/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/enfants/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<EnfantDto>();
            updated.ShouldNotBeNull();
            updated!.Nom.ShouldBe("Emma Modifiée");
        }

        [Fact]
        public async Task UpdateEnfant_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateEnfantDto
            {
                Id = 2,
                Nom = "Invalid",
                Civilite = "m"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/enfants/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateEnfant_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateEnfantDto
            {
                Id = 9999,
                Nom = "Inconnu",
                Civilite = "autre"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/enfants/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task DeleteEnfant_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateEnfantDto { Nom = "Lucas", Civilite = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<EnfantDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/enfants/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/enfants/{created.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteEnfant_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/v1/enfants/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }
    }
}