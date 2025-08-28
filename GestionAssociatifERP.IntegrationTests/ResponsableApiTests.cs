using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class ResponsableApiTests
    {
        [Fact]
        public async Task GetAllResponsables_ShouldReturnResponsables_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/responsables";
            var dto = new CreateResponsableDto { Nom = "Alice" };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var responsables = await response.Content.ReadFromJsonAsync<List<ResponsableDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldContain(e => e.Nom == "Alice");
        }

        [Fact]
        public async Task GetAllResponsables_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/responsables";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var responsables = await response.Content.ReadFromJsonAsync<List<ResponsableDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetResponsableById_ShouldReturnResponsable_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateResponsableDto { Nom = "Bob" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/responsables", createDto);
            postResponse.EnsureSuccessStatusCode();

            var createdResponsable = await postResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var responsable = await response.Content.ReadFromJsonAsync<ResponsableDto>();
            responsable.ShouldNotBeNull();
            responsable.Id.ShouldBe(createdResponsable.Id);
            responsable.Nom.ShouldBe("Bob");
        }

        [Fact]
        public async Task GetResponsableById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/responsables/999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithEnfants__ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var responsableDto = new CreateResponsableDto { Nom = "Dupont", Civilite = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            // 2. Créer un enfant
            var enfantDto = new CreateEnfantDto { Nom = "Alice", Civilite = "mme" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<EnfantDto>();

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
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ResponsableWithEnfantsDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.Enfants.ShouldNotBeNull();
            result.Enfants.ShouldContain(r => r.Nom == "Alice");
        }

        [Fact]
        public async Task GetWithEnfants_ShouldReturnEmptyList_WhenNoEnfants()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsableDto = new CreateResponsableDto { Nom = "Martin", Civilite = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();

            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ResponsableWithEnfantsDto>();
            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.Enfants.ShouldNotBeNull();
            result.Enfants.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetWithEnfants_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/responsables/9999/with-enfants");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithInformationFinanciere_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var responsableDto = new CreateResponsableDto { Nom = "Lefevre", Civilite = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            // 2. Créer une information financière
            var infoFinanciereDto = new CreateInformationFinanciereDto
            {
                ResponsableId = createdResponsable!.Id,
                QuotientFamiliale = 100,
                DateDebut = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            };
            var postInfoFinanciere = await client.PostAsJsonAsync("/api/v1/informationsfinancieres", infoFinanciereDto);
            postInfoFinanciere.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-information-financiere");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ResponsableWithInformationFinanciereDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.InformationFinanciere.ShouldNotBeNull();
            result.InformationFinanciere.QuotientFamiliale.ShouldBe(100);
        }

        [Fact]
        public async Task GetWithInformationFinanciere_ShouldReturnEmpty_WhenNoInformationFinanciere()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsableDto = new CreateResponsableDto { Nom = "Durand", Civilite = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-information-financiere");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ResponsableWithInformationFinanciereDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.InformationFinanciere.ShouldBeNull();
        }

        [Fact]
        public async Task GetWithInformationFinanciere_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/responsables/9999/with-information-financiere");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithSituationPersonnelle_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var responsableDto = new CreateResponsableDto { Nom = "Bernard", Civilite = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            // 2. Créer une situation personnelle
            var situationPersonnelleDto = new CreateSituationPersonnelleDto
            {
                ResponsableId = createdResponsable!.Id,
                SituationFamiliale = "Célibataire",
                Secteur = "Test"
            };
            var postSituationPersonnelle = await client.PostAsJsonAsync("/api/v1/situationspersonnelles", situationPersonnelleDto);
            postSituationPersonnelle.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-situation-personnelle");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ResponsableWithSituationPersonnelleDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.SituationPersonnelle.ShouldNotBeNull();
            result.SituationPersonnelle.SituationFamiliale.ShouldBe("Célibataire");
        }

        [Fact]
        public async Task GetWithSituationPersonnelle_ShouldReturnEmpty_WhenNoSituationPersonnelle()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsableDto = new CreateResponsableDto { Nom = "Lemoine", Civilite = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();

            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-situation-personnelle");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ResponsableWithSituationPersonnelleDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.SituationPersonnelle.ShouldBeNull();
        }

        [Fact]
        public async Task GetWithSituationPersonnelle_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/responsables/9999/with-situation-personnelle");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateResponsable_ShouldReturnCreated_WhenValidData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateResponsableDto { Nom = "Martin", Civilite = "m" };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/responsables", createDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var createdResponsable = await response.Content.ReadFromJsonAsync<ResponsableDto>();
            createdResponsable.ShouldNotBeNull();
            createdResponsable.Nom.ShouldBe("Martin");
            createdResponsable.Civilite.ShouldBe("m");
        }

        [Fact]
        public async Task UpdateResponsable_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var createDto = new CreateResponsableDto { Nom = "Dupuis", Civilite = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/responsables", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdResponsable = await postResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            // 2. Mettre à jour le responsable
            var updateDto = new UpdateResponsableDto
            {
                Id = createdResponsable!.Id,
                Nom = "Dupuis Modifié",
                Civilite = "mme"
            };

            // Act
            var updateResponse = await client.PutAsJsonAsync($"/api/v1/responsables/{createdResponsable.Id}", updateDto);

            // Assert
            updateResponse.EnsureSuccessStatusCode();
            updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/responsables/{createdResponsable.Id}");
            var updatedResponsable = await getResponse.Content.ReadFromJsonAsync<ResponsableDto>();
            updatedResponsable.ShouldNotBeNull();
            updatedResponsable.Nom.ShouldBe("Dupuis Modifié");
        }

        [Fact]
        public async Task UpdateResponsable_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateResponsableDto
            {
                Id = 2,
                Nom = "Lemoine Modifié",
                Civilite = "mme"
            };

            // Act
            var updateResponse = await client.PutAsJsonAsync($"/api/v1/responsables/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(updateResponse, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant du responsable ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateResponsable_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateResponsableDto
            {
                Id = 9999,
                Nom = "Inconnu",
                Civilite = "m"
            };

            // Act
            var updateResponse = await client.PutAsJsonAsync($"/api/v1/responsables/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(updateResponse, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task DeleteResponsable_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateResponsableDto { Nom = "Garnier", Civilite = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/responsables", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdResponsable = await postResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            // Act
            var deleteResponse = await client.DeleteAsync($"/api/v1/responsables/{createdResponsable!.Id}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/responsables/{createdResponsable.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteResponsable_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var deleteResponse = await client.DeleteAsync("/api/v1/responsables/9999");
            var exception = await AssertProblemDetails.AssertProblem(deleteResponse, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }
    }
}