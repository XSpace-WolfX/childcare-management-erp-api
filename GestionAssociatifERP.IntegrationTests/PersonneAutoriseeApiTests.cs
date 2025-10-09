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
            var dto = new CreateAuthorizedPersonDto { LastName = "Test", FirstName = "User" };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personnesAutorisees = await response.Content.ReadFromJsonAsync<List<AuthorizedPersonDto>>();
            personnesAutorisees.ShouldNotBeNull();
            personnesAutorisees.ShouldContain(e => e.LastName == "Test");
            personnesAutorisees.ShouldContain(e => e.FirstName == "User");
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

            var personnesAutorisees = await response.Content.ReadFromJsonAsync<List<AuthorizedPersonDto>>();
            personnesAutorisees.ShouldNotBeNull();
            personnesAutorisees.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonneAutoriseeById_ReturnsOkResult_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAuthorizedPersonDto { LastName = "Test", FirstName = "User" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", createDto);
            postResponse.EnsureSuccessStatusCode();

            var createdPersonne = await postResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonne!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personneAutorisee = await response.Content.ReadFromJsonAsync<AuthorizedPersonDto>();
            personneAutorisee.ShouldNotBeNull();
            personneAutorisee.Id.ShouldBe(createdPersonne.Id);
            personneAutorisee.LastName.ShouldBe("Test");
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
            var personneAutoriseeDto = new CreateAuthorizedPersonDto { LastName = "Alice", FirstName = "mam" };
            var postPersonneAutorisee = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutoriseeDto);
            postPersonneAutorisee.EnsureSuccessStatusCode();
            var createdPersonneAutorisee = await postPersonneAutorisee.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // 2. Créer un enfant
            var enfantDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<ChildDto>();

            // 3. Lier personne autorisée ↔ enfant
            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdEnfant!.Id,
                AuthorizedPersonId = createdPersonneAutorisee!.Id,
                Relationship = "Parent"
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonneAutorisee!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthorizedPersonWithChildrenDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdPersonneAutorisee.Id);
            result.Children.ShouldNotBeNull();
            result.Children.ShouldContain(r => r.LastName == "Alice");
        }

        [Fact]
        public async Task GetWithEnfants_ShouldReturnEmptyList_WhenNoEnfants()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer une personne autorisée
            var personneAutoriseeDto = new CreateAuthorizedPersonDto { LastName = "Bob", FirstName = "Smith" };
            var postPersonneAutorisee = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutoriseeDto);
            postPersonneAutorisee.EnsureSuccessStatusCode();

            var createdPersonneAutorisee = await postPersonneAutorisee.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonneAutorisee!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthorizedPersonWithChildrenDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdPersonneAutorisee.Id);
            result.Children.ShouldNotBeNull();
            result.Children.ShouldBeEmpty();
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

            var dto = new CreateAuthorizedPersonDto { LastName = "John", FirstName = "Doe", Phone = "123456789" };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/personnesautorisees", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var createdPersonne = await response.Content.ReadFromJsonAsync<AuthorizedPersonDto>();
            createdPersonne.ShouldNotBeNull();
            createdPersonne.LastName.ShouldBe("John");
            createdPersonne.FirstName.ShouldBe("Doe");
        }

        [Fact]
        public async Task UpdatePersonneAutorisee_ShouldReturnOk_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAuthorizedPersonDto { LastName = "Jane", FirstName = "Doe" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdPersonne = await postResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var updateDto = new UpdateAuthorizedPersonDto
            {
                Id = createdPersonne!.Id,
                LastName = "Jane Updated",
                FirstName = "Doe Updated"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/personnesautorisees/{createdPersonne.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/personnesautorisees/{createdPersonne.Id}");
            var updatedPersonne = await getResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();
            updatedPersonne.ShouldNotBeNull();
            updatedPersonne.LastName.ShouldBe("Jane Updated");
            updatedPersonne.FirstName.ShouldBe("Doe Updated");
        }

        [Fact]
        public async Task UpdatePersonneAutorisee_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateAuthorizedPersonDto
            {
                Id = 2,
                LastName = "Jane Updated",
                FirstName = "Doe Updated"
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

            var updateDto = new UpdateAuthorizedPersonDto
            {
                Id = 999, // ID qui n'existe pas
                LastName = "Non Existent",
                FirstName = "User"
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

            var createDto = new CreateAuthorizedPersonDto { LastName = "Delete", FirstName = "Me" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdPersonne = await postResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

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