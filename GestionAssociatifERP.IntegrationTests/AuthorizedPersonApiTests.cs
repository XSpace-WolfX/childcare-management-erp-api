using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class AuthorizedPersonApiTests
    {
        [Fact]
        public async Task GetAllAuthorizedPeople_ReturnsOkResult()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/authorizedpeople";
            var AuthorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Test", FirstName = "User" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync(url, AuthorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var authorizedPeople = await response.Content.ReadFromJsonAsync<List<AuthorizedPersonDto>>();
            authorizedPeople.ShouldNotBeNull();
            authorizedPeople.ShouldContain(ap => ap.LastName == "Test");
            authorizedPeople.ShouldContain(ap => ap.FirstName == "User");
        }

        [Fact]
        public async Task GetAllAuthorizedPeople_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/authorizedpeople";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var authorizedPeople = await response.Content.ReadFromJsonAsync<List<AuthorizedPersonDto>>();
            authorizedPeople.ShouldNotBeNull();
            authorizedPeople.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAuthorizedPersonById_ReturnsOkResult_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Test", FirstName = "User" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/authorizedpeople/{createdAuthorizedPerson!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var authorizedPerson = await response.Content.ReadFromJsonAsync<AuthorizedPersonDto>();
            authorizedPerson.ShouldNotBeNull();
            authorizedPerson.Id.ShouldBe(createdAuthorizedPerson.Id);
            authorizedPerson.LastName.ShouldBe("Test");
        }

        [Fact]
        public async Task GetAuthorizedPersonById_ReturnsNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/authorizedpeople/999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task GetAuthorizedPersonWithChildren_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer une personne autorisée
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Alice", FirstName = "mam" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // 2. Créer un enfant
            var childDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // 3. Lier personne autorisée ↔ enfant
            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "Parent"
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/authorizedpeople/{createdAuthorizedPerson!.Id}/with-children");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthorizedPersonWithChildrenDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdAuthorizedPerson.Id);
            result.Children.ShouldNotBeNull();
            result.Children.ShouldContain(c => c.LastName == "Alice");
        }

        [Fact]
        public async Task GetAuthorizedPersonWithChildren_ShouldReturnEmptyList_WhenNoChild()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer une personne autorisée
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Bob", FirstName = "Smith" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/authorizedpeople/{createdAuthorizedPerson!.Id}/with-children");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<AuthorizedPersonWithChildrenDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdAuthorizedPerson.Id);
            result.Children.ShouldNotBeNull();
            result.Children.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAuthorizedPersonWithChildren_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/authorizedpeople/999/with-children");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateAuthorizedPerson_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var AuthorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "John", FirstName = "Doe", Phone = "123456789" };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/authorizedpeople", AuthorizedPersonDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var authorizedPerson = await response.Content.ReadFromJsonAsync<AuthorizedPersonDto>();
            authorizedPerson.ShouldNotBeNull();
            authorizedPerson.LastName.ShouldBe("John");
            authorizedPerson.FirstName.ShouldBe("Doe");
        }

        [Fact]
        public async Task UpdateAuthorizedPerson_ShouldReturnOk_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Jane", FirstName = "Doe" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var updateDto = new UpdateAuthorizedPersonDto
            {
                Id = createdAuthorizedPerson!.Id,
                LastName = "Jane Updated",
                FirstName = "Doe Updated"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/authorizedpeople/{createdAuthorizedPerson.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/authorizedpeople/{createdAuthorizedPerson.Id}");
            var updatedPersonne = await getResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();
            updatedPersonne.ShouldNotBeNull();
            updatedPersonne.LastName.ShouldBe("Jane Updated");
            updatedPersonne.FirstName.ShouldBe("Doe Updated");
        }

        [Fact]
        public async Task UpdateAuthorizedPerson_ShouldReturnBadRequest_WhenIdMismatch()
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
            var response = await client.PutAsJsonAsync("/api/v1/authorizedpeople/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de la personne autorisée ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateAuthorizedPerson_ShouldReturnNotFound_WhenDoesNotExist()
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
            var response = await client.PutAsJsonAsync("/api/v1/authorizedpeople/999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task DeleteAuthorizedPerson_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Delete", FirstName = "Me" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/authorizedpeople/{createdAuthorizedPerson!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/authorizedpeople/{createdAuthorizedPerson.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAuthorizedPerson_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/authorizedpeople/999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }
    }
}