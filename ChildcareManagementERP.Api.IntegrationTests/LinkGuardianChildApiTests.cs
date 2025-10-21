using ChildcareManagementERP.Api.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ChildcareManagementERP.Api.IntegrationTests
{
    public class LinkGuardianChildApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        [Fact]
        public async Task GetGuardiansByChildId_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Lina", Gender = "mme" };
            var guardianDto = new CreateGuardianDto { LastName = "Sophie", Title = "mme" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Mère"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkguardianchild/child/{createdChild.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getGuardians = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            getGuardians.ShouldNotBeNull();
            getGuardians.ShouldContain(r => r.GuardianId == createdGuardian.Id);
        }

        [Fact]
        public async Task GetGuardiansByChildId_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Noa", Gender = "non_specifie" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkguardianchild/child/{createdChild!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getGuardians = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            getGuardians.ShouldNotBeNull();
            getGuardians.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetGuardiansByChildId_ShouldReturnNotFound_WhenChildDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkguardianchild/child/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetChildrenByGuardianId_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Émile", Gender = "m" };
            var guardianDto = new CreateGuardianDto { LastName = "Nathalie", Title = "mme" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Tante"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkguardianchild/guardian/{createdGuardian.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getChildren = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            getChildren.ShouldNotBeNull();
            getChildren.ShouldContain(e => e.ChildId == createdChild.Id);
        }

        [Fact]
        public async Task GetChildrenByGuardianId_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var guardianDto = new CreateGuardianDto { LastName = "Julie", Title = "mme" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkguardianchild/guardian/{createdGuardian!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var getChildren = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            getChildren.ShouldNotBeNull();
            getChildren.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildrenByGuardianId_ShouldReturnNotFound_WhenGuardianDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkguardianchild/guardian/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task ExistsLinkGuardianChild_ShouldReturnTrue_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Élise", Gender = "mme" };
            var guardianDto = new CreateGuardianDto { LastName = "Paul", Title = "m" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Mère"
            };

            var postLink = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            postLink.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkguardianchild/guardian/{createdGuardian.Id}/child/{createdChild.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var exists = await response.Content.ReadFromJsonAsync<bool>();
            exists.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkGuardianChild_ShouldReturnFalse_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Hugo", Gender = "m" };
            var guardianDto = new CreateGuardianDto { LastName = "Claire", Title = "mme" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkguardianchild/guardian/{createdGuardian!.Id}/child/{createdChild!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var exists = await response.Content.ReadFromJsonAsync<bool>();
            exists.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkGuardianChild_ShouldReturnOk_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Camille", Gender = "mme" };
            var guardianDto = new CreateGuardianDto { LastName = "Jean", Title = "m" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var dto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Père"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkguardianchild", dto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LinkGuardianChildDto>();
            result.ShouldNotBeNull();
            result.ChildId.ShouldBe(dto.ChildId);
            result.GuardianId.ShouldBe(dto.GuardianId);
            result.Relationship.ShouldBe("Père");
        }

        [Fact]
        public async Task CreateLinkGuardianChild_ShouldReturnNotFound_WhenChildDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Étape 1 – Créer un responsable valide
            var guardianDto = new CreateGuardianDto
            {
                LastName = "Test",
                FirstName = "Responsable",
                Email = "test.responsable@example.com",
                Phone = "0600000001"
            };

            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Étape 2 – EnfantId inexistant
            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = 9999,
                GuardianId = createdGuardian!.Id,
                Relationship = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkGuardianChild_ShouldReturnNotFound_WhenGuardianDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Étape 1 – Créer un enfant valide
            var childDto = new CreateChildDto
            {
                LastName = "Test",
                FirstName = "Enfant",
                BirthDate = DateOnly.FromDateTime(DateTime.Today.AddYears(-6)),
                Gender = "F"
            };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Étape 2 – ResponsableId inexistant
            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = 9999,
                Relationship = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkGuardianChild_ShouldReturnConflict_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Julien", Gender = "m" };
            var guardianDto = new CreateGuardianDto { LastName = "Lucie", Title = "mme" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Mère"
            };

            // First creation
            var firstResponse = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            // Act - Second creation should fail
            var secondResponse = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(secondResponse, HttpStatusCode.Conflict);

            // Assert
            exception.Detail.ShouldBe("Ce lien existe déjà entre ce responsable et cet enfant.");
        }

        [Fact]
        public async Task UpdateLinkGuardianChild_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();
            
            var childDto = new CreateChildDto { LastName = "Axel", Gender = "m" };
            var guardianDto = new CreateGuardianDto { LastName = "Marine", Title = "mme" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Tante"
            };

            var postLink = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            postLink.EnsureSuccessStatusCode();

            var updateDto = new UpdateLinkGuardianChildDto
            {
                ChildId = linkDto.ChildId,
                GuardianId = linkDto.GuardianId,
                Relationship = "Tuteur"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/linkguardianchild", updateDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateLinkGuardianChild_ShouldReturnNotFound_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateLinkGuardianChildDto
            {
                ChildId = 999,
                GuardianId = 888,
                Relationship = "Tuteur"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/linkguardianchild", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun lien Responsable / Enfant trouvé à mettre à jour.");
        }

        [Fact]
        public async Task DeleteLinkGuardianChild_ShouldReturnNoContent_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Lucas", Gender = "m" };
            var guardianDto = new CreateGuardianDto { LastName = "Claire", Title = "mme" };

            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postChildResponse.EnsureSuccessStatusCode();
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Père"
            };

            var postLink = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            postLink.EnsureSuccessStatusCode();

            // Act
            var response = await client.DeleteAsync($"/api/v1/linkguardianchild/guardian/{createdGuardian.Id}/child/{createdChild.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteLinkGuardianChild_ShouldReturnNotFound_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/linkguardianchild/guardian/9999/child/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun lien Responsable / Enfant trouvé à supprimer.");
        }
    }
}