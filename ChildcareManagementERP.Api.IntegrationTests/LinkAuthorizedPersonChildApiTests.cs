using ChildcareManagementERP.Api.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ChildcareManagementERP.Api.IntegrationTests
{
    public class LinkAuthorizedPersonChildApiTests
    {
        [Fact]
        public async Task GetChildrenByAuthorizedPerson_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();


            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkauthorizedpersonchild/authorized-person/{createdAuthorizedPerson.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getChildren = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            getChildren.ShouldNotBeNull();
            getChildren.ShouldContain(r => r.ChildId == createdChild.Id);
        }

        [Fact]
        public async Task GetChildrenByAuthorizedPerson_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Noa", FirstName = "non_specifie" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkauthorizedpersonchild/authorized-person/{createdAuthorizedPerson!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getChildren = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            getChildren.ShouldNotBeNull();
            getChildren.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildrenByAuthorizedPerson_ShouldReturnNotFound_WhenAuthorizedPersonDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkauthorizedpersonchild/authorized-person/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("La personne autorisée spécifiée n'existe pas.");
        }

        [Fact]
        public async Task GetAuthorizedPeopleByChild_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkauthorizedpersonchild/child/{createdChild.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getAuthorizedPeople = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            getAuthorizedPeople.ShouldNotBeNull();
            getAuthorizedPeople.ShouldContain(r => r.AuthorizedPersonId == createdAuthorizedPerson.Id);
        }

        [Fact]
        public async Task GetAuthorizedPeopleByChild_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Noa", FirstName = "non_specifie" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkauthorizedpersonchild/child/{createdChild!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getAuthorizedPeople = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            getAuthorizedPeople.ShouldNotBeNull();
            getAuthorizedPeople.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAuthorizedPeopleByChild_ShouldReturnNotFound_WhenChildDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkauthorizedpersonchild/child/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task ExistsLinkAuthorizedPersonChild_ShouldReturnTrue_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkauthorizedpersonchild/authorized-person/{createdAuthorizedPerson.Id}/child/{createdChild.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var existsResult = await response.Content.ReadFromJsonAsync<bool>();
            existsResult.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkAuthorizedPersonChild_ShouldReturnFalse_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkauthorizedpersonchild/authorized-person/{createdAuthorizedPerson!.Id}/child/{createdChild!.Id}");
            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var existsResult = await response.Content.ReadFromJsonAsync<bool>();
            existsResult.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChild_ShouldReturnOk_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LinkAuthorizedPersonChildDto>();
            result.ShouldNotBeNull();
            result.ChildId.ShouldBe(createdChild.Id);
            result.AuthorizedPersonId.ShouldBe(createdAuthorizedPerson.Id);
            result.Relationship.ShouldBe("TestAffiliation");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChild_ShouldReturnNotFound_WhenAuthorizedPersonDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto
            {
                LastName = "Test",
                FirstName = "Enfant",
                BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-5)),
                Gender = "M"
            };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = 9999, // Non-existent Personne Autorisee ID
                Relationship = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("La personne autorisée spécifiée n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChild_ShouldReturnNotFound_WhenChildDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var authorizedPersonDto = new CreateAuthorizedPersonDto
            {
                LastName = "Test",
                FirstName = "Autorisee",
                Phone = "0600000000"
            };

            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 9999, // Non-existent Enfant ID
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChild_ShouldReturnConflict_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            // First creation
            var firstResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            firstResponse.EnsureSuccessStatusCode();

            // Act - Second creation should fail
            var secondResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(secondResponse, HttpStatusCode.Conflict);

            // Assert
            exception.Detail.ShouldBe("Ce lien existe déjà entre cette personne autorisée et cet enfant.");
        }

        [Fact]
        public async Task UpdateLinkAuthorizedPersonChild_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            var createResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            createResponse.EnsureSuccessStatusCode();

            // Update DTO
            var updateDto = new UpdateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild.Id,
                AuthorizedPersonId = createdAuthorizedPerson.Id,
                Relationship = "UpdatedAffiliation"
            };

            // Act
            var updateResponse = await client.PutAsJsonAsync("/api/v1/linkauthorizedpersonchild", updateDto);

            // Assert
            updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateLinkAuthorizedPersonChild_ShouldReturnNotFound_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateLinkAuthorizedPersonChildDto
            {
                ChildId = 9999,
                AuthorizedPersonId = 9999,
                Relationship = "UpdatedAffiliation"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/linkauthorizedpersonchild", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas.");
        }

        [Fact]
        public async Task DeleteLinkAuthorizedPersonChild_ShouldReturnNoContent_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postChildResponse.EnsureSuccessStatusCode();
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id,
                Relationship = "TestAffiliation"
            };

            var createResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            createResponse.EnsureSuccessStatusCode();

            // Act
            var deleteResponse = await client.DeleteAsync($"/api/v1/linkauthorizedpersonchild/authorized-person/{createdAuthorizedPerson.Id}/child/{createdChild.Id}");

            // Assert
            deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteLinkAuthorizedPersonChild_ShouldReturnNotFound_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/linkauthorizedpersonchild/authorized-person/9999/child/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas.");
        }
    }
}