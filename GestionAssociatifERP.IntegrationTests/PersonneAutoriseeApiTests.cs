using GestionAssociatifERP.Dtos.V1;
using Microsoft.AspNetCore.Http.HttpResults;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class PersonneAutoriseeApiTests
    {
        [Fact]
        public async Task GetAllPersonnesAutorisees_ReturnsOkResult()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/personnesautorisees";
            var dto = new CreatePersonneAutoriseeDto { Nom = "Test", Prenom = "User" };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personnesAutorisees = await response.Content.ReadFromJsonAsync<List<PersonneAutoriseeDto>>();
            personnesAutorisees.ShouldNotBeNull();
            personnesAutorisees.ShouldContain(e => e.Nom == "Test");
            personnesAutorisees.ShouldContain(e => e.Prenom == "User");
        }

        [Fact]
        public async Task GetAllPersonnesAutorisees_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/personnesautorisees";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personnesAutorisees = await response.Content.ReadFromJsonAsync<List<PersonneAutoriseeDto>>();
            personnesAutorisees.ShouldNotBeNull();
            personnesAutorisees.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonneAutoriseeById_ReturnsOkResult_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreatePersonneAutoriseeDto { Nom = "Test", Prenom = "User" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", createDto);
            postResponse.EnsureSuccessStatusCode();

            var createdPersonne = await postResponse.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonne!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personneAutorisee = await response.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();
            personneAutorisee.ShouldNotBeNull();
            personneAutorisee.Id.ShouldBe(createdPersonne.Id);
            personneAutorisee.Nom.ShouldBe("Test");
        }

        [Fact]
        public async Task GetPersonneAutoriseeById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/personnesautorisees/999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task GetWithEnfants_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer une personne autorisée
            var personneAutoriseeDto = new CreatePersonneAutoriseeDto { Nom = "Alice", Prenom = "mam" };
            var postPersonneAutorisee = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutoriseeDto);
            postPersonneAutorisee.EnsureSuccessStatusCode();
            var createdPersonneAutorisee = await postPersonneAutorisee.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();

            // 2. Créer un enfant
            var enfantDto = new CreateEnfantDto { Nom = "Alice", Civilite = "mme" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<EnfantDto>();

            // 3. Lier personne autorisée ↔ enfant
            var linkDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = createdEnfant!.Id,
                PersonneAutoriseeId = createdPersonneAutorisee!.Id,
                Affiliation = "Parent"
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonneAutorisee!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PersonneAutoriseeWithEnfantsDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdPersonneAutorisee.Id);
            result.Enfants.ShouldNotBeNull();
            result.Enfants.ShouldContain(r => r.Nom == "Alice");
        }

        [Fact]
        public async Task GetWithEnfants_ShouldReturnEmptyList_WhenNoEnfants()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer une personne autorisée
            var personneAutoriseeDto = new CreatePersonneAutoriseeDto { Nom = "Bob", Prenom = "Smith" };
            var postPersonneAutorisee = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutoriseeDto);
            postPersonneAutorisee.EnsureSuccessStatusCode();

            var createdPersonneAutorisee = await postPersonneAutorisee.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonneAutorisee!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<PersonneAutoriseeWithEnfantsDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdPersonneAutorisee.Id);
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
            var response = await client.GetAsync("/api/v1/personnesautorisees/999/with-enfants");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreatePersonneAutorisee_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new CreatePersonneAutoriseeDto { Nom = "John", Prenom = "Doe", Telephone = "123456789" };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/personnesautorisees", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var createdPersonne = await response.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();
            createdPersonne.ShouldNotBeNull();
            createdPersonne.Nom.ShouldBe("John");
            createdPersonne.Prenom.ShouldBe("Doe");
        }

        [Fact]
        public async Task UpdatePersonneAutorisee_ShouldReturnOk_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreatePersonneAutoriseeDto { Nom = "Jane", Prenom = "Doe" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdPersonne = await postResponse.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();

            var updateDto = new UpdatePersonneAutoriseeDto
            {
                Id = createdPersonne!.Id,
                Nom = "Jane Updated",
                Prenom = "Doe Updated"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/personnesautorisees/{createdPersonne.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonne.Id}");
            var updatedPersonne = await getResponse.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();
            updatedPersonne.ShouldNotBeNull();
            updatedPersonne.Nom.ShouldBe("Jane Updated");
            updatedPersonne.Prenom.ShouldBe("Doe Updated");
        }

        [Fact]
        public async Task UpdatePersonneAutorisee_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdatePersonneAutoriseeDto
            {
                Id = 2,
                Nom = "Jane Updated",
                Prenom = "Doe Updated"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/personnesautorisees/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de la personne autorisée ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdatePersonneAutorisee_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdatePersonneAutoriseeDto
            {
                Id = 999, // ID qui n'existe pas
                Nom = "Non Existent",
                Prenom = "User"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/personnesautorisees/999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task DeletePersonneAutorisee_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreatePersonneAutoriseeDto { Nom = "Delete", Prenom = "Me" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdPersonne = await postResponse.Content.ReadFromJsonAsync<PersonneAutoriseeDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/personnesautorisees/{createdPersonne!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonne.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeletePersonneAutorisee_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/personnesautorisees/999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }
    }
}