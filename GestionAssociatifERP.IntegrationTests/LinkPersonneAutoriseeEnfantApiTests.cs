using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class LinkPersonneAutoriseeEnfantApiTests
    {
        [Fact]
        public async Task GetEnfantsByPersonneAutorisee_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();


            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated!.Id,
                AuthorizedPersonId = personneAutoriseeCreated!.Id,
                Relationship = "TestAffiliation"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkpersonneautoriseeenfant/personne-autorisee/{personneAutoriseeCreated.Id}");
            var raw = await response.Content.ReadAsStringAsync();
            Console.WriteLine($"Erreur : {response.StatusCode}, Body: {raw}");
            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var responsables = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldContain(r => r.ChildId == enfantCreated.Id);
        }

        [Fact]
        public async Task GetEnfantsByPersonneAutorisee_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "Noa", FirstName = "non_specifie" };
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            personneAutoriseeResponse.EnsureSuccessStatusCode();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkpersonneautoriseeenfant/personne-autorisee/{personneAutoriseeCreated!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var enfants = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantsByPersonneAutorisee_ShouldReturnNotFound_WhenPersonneAutoriseeDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkpersonneautoriseeenfant/personne-autorisee/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetPersonnesByEnfant_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated!.Id,
                AuthorizedPersonId = personneAutoriseeCreated!.Id,
                Relationship = "TestAffiliation"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkpersonneautoriseeenfant/enfant/{enfantCreated.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var personnesAutorisees = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            personnesAutorisees.ShouldNotBeNull();
            personnesAutorisees.ShouldContain(r => r.AuthorizedPersonId == personneAutoriseeCreated.Id);
        }

        [Fact]
        public async Task GetPersonnesByEnfant_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Noa", FirstName = "non_specifie" };
            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            enfantResponse.EnsureSuccessStatusCode();
            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkpersonneautoriseeenfant/enfant/{enfantCreated!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var personnesAutorisees = await response.Content.ReadFromJsonAsync<List<LinkAuthorizedPersonChildDto>>();
            personnesAutorisees.ShouldNotBeNull();
            personnesAutorisees.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonnesByEnfant_ShouldReturnNotFound_WhenEnfantDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkpersonneautoriseeenfant/enfant/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistsLinkPersonneAutoriseeEnfant_ShouldReturnTrue_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated!.Id,
                AuthorizedPersonId = personneAutoriseeCreated!.Id,
                Relationship = "TestAffiliation"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkpersonneautoriseeenfant/personne-autorisee/{personneAutoriseeCreated.Id}/enfant/{enfantCreated.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var existsResult = await response.Content.ReadFromJsonAsync<bool>();
            existsResult.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkPersonneAutoriseeEnfant_ShouldReturnFalse_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkpersonneautoriseeenfant/personne-autorisee/{personneAutoriseeCreated!.Id}/enfant/{enfantCreated!.Id}");
            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var existsResult = await response.Content.ReadFromJsonAsync<bool>();
            existsResult.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfant_ShouldReturnOk_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated!.Id,
                AuthorizedPersonId = personneAutoriseeCreated!.Id,
                Relationship = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LinkAuthorizedPersonChildDto>();
            result.ShouldNotBeNull();
            result.ChildId.ShouldBe(enfantCreated.Id);
            result.AuthorizedPersonId.ShouldBe(personneAutoriseeCreated.Id);
            result.Relationship.ShouldBe("TestAffiliation");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfant_ShouldReturnNotFound_WhenEnfantOrPersonneAutoriseeDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 9999, // Non-existent Enfant ID
                AuthorizedPersonId = 9999, // Non-existent Personne Autorisee ID
                Relationship = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfant_ShouldReturnConflict_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated!.Id,
                AuthorizedPersonId = personneAutoriseeCreated!.Id,
                Relationship = "TestAffiliation"
            };

            // First creation
            var firstResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            firstResponse.EnsureSuccessStatusCode();

            // Act - Second creation should fail
            var secondResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);

            // Assert
            secondResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task UpdateLinkPersonneAutoriseeEnfant_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated!.Id,
                AuthorizedPersonId = personneAutoriseeCreated!.Id,
                Relationship = "TestAffiliation"
            };

            var createResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            createResponse.EnsureSuccessStatusCode();

            // Update DTO
            var updateDto = new UpdateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated.Id,
                AuthorizedPersonId = personneAutoriseeCreated.Id,
                Relationship = "UpdatedAffiliation"
            };

            // Act
            var updateResponse = await client.PutAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", updateDto);

            // Assert
            updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateLinkPersonneAutoriseeEnfant_ShouldReturnNotFound_WhenLinkDoesNotExist()
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
            var response = await client.PutAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", updateDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteLinkPersonneAutoriseeEnfant_ShouldReturnNoContent_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "TestEnfant", FirstName = "Enfant" };
            var personneAutorisee = new CreateAuthorizedPersonDto { LastName = "TestAutorisee", FirstName = "Autorisee" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var personneAutoriseeResponse = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneAutorisee);
            enfantResponse.EnsureSuccessStatusCode();
            personneAutoriseeResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var personneAutoriseeCreated = await personneAutoriseeResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = enfantCreated!.Id,
                AuthorizedPersonId = personneAutoriseeCreated!.Id,
                Relationship = "TestAffiliation"
            };

            var createResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            createResponse.EnsureSuccessStatusCode();

            // Act
            var deleteResponse = await client.DeleteAsync($"/api/v1/linkpersonneautoriseeenfant/personne-autorisee/{personneAutoriseeCreated.Id}/enfant/{enfantCreated.Id}");

            // Assert
            deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteLinkPersonneAutoriseeEnfant_ShouldReturnNotFound_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/linkpersonneautoriseeenfant/personne-autorisee/9999/enfant/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}