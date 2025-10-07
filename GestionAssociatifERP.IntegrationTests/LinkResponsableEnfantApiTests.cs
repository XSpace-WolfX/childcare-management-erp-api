using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class LinkResponsableEnfantApiTests : IClassFixture<CustomWebApplicationFactory>
    {
        [Fact]
        public async Task GetResponsablesByEnfantId_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Lina", Gender = "mme" };
            var responsable = new CreateGuardianDto { LastName = "Sophie", Title = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = enfantCreated!.Id,
                GuardianId = responsableCreated!.Id,
                Relationship = "Mère"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/enfant/{enfantCreated.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var responsables = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldContain(r => r.GuardianId == responsableCreated.Id);
        }

        [Fact]
        public async Task GetResponsablesByEnfantId_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Noa", Gender = "non_specifie" };
            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            enfantResponse.EnsureSuccessStatusCode();
            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/enfant/{enfantCreated!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var responsables = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetResponsablesByEnfantId_ShouldReturnNotFound_WhenEnfantDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkresponsableenfant/enfant/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task GetEnfantsByResponsableId_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Émile", Gender = "m" };
            var responsable = new CreateGuardianDto { LastName = "Nathalie", Title = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = enfantCreated!.Id,
                GuardianId = responsableCreated!.Id,
                Relationship = "Tante"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/responsable/{responsableCreated.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var enfants = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldContain(e => e.ChildId == enfantCreated.Id);
        }

        [Fact]
        public async Task GetEnfantsByResponsableId_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsable = new CreateGuardianDto { LastName = "Julie", Title = "mme" };
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            responsableResponse.EnsureSuccessStatusCode();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/responsable/{responsableCreated!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var enfants = await response.Content.ReadFromJsonAsync<List<LinkGuardianChildDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantsByResponsableId_ShouldReturnNotFound_WhenResponsableDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/linkresponsableenfant/responsable/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task ExistsLinkResponsableEnfant_ShouldReturnTrue_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Élise", Gender = "mme" };
            var responsable = new CreateGuardianDto { LastName = "Paul", Title = "m" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = enfantCreated!.Id,
                GuardianId = responsableCreated!.Id,
                Relationship = "Mère"
            };

            var postLink = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            postLink.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/responsable/{responsableCreated.Id}/enfant/{enfantCreated.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var exists = await response.Content.ReadFromJsonAsync<bool>();
            exists.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkResponsableEnfant_ShouldReturnFalse_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Hugo", Gender = "m" };
            var responsable = new CreateGuardianDto { LastName = "Claire", Title = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/responsable/{responsableCreated!.Id}/enfant/{enfantCreated!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var exists = await response.Content.ReadFromJsonAsync<bool>();
            exists.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkResponsableEnfant_ShouldReturnOk_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Camille", Gender = "mme" };
            var responsable = new CreateGuardianDto { LastName = "Jean", Title = "m" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var dto = new CreateLinkGuardianChildDto
            {
                ChildId = enfantCreated!.Id,
                GuardianId = responsableCreated!.Id,
                Relationship = "Père"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", dto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LinkGuardianChildDto>();
            result.ShouldNotBeNull();
            result.ChildId.ShouldBe(dto.ChildId);
            result.GuardianId.ShouldBe(dto.GuardianId);
            result.Relationship.ShouldBe("Père");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfant_ShouldReturnConflict_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Julien", Gender = "m" };
            var responsable = new CreateGuardianDto { LastName = "Lucie", Title = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<ChildDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<GuardianDto>();

            var dto = new CreateLinkGuardianChildDto
            {
                ChildId = enfantCreated!.Id,
                GuardianId = responsableCreated!.Id,
                Relationship = "Mère"
            };

            // First creation
            var firstResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", dto);
            firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            // Act - Second creation should fail
            var secondResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", dto);

            // Assert
            secondResponse.StatusCode.ShouldBe(HttpStatusCode.Conflict);
        }

        [Fact]
        public async Task UpdateLinkResponsableEnfant_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();
            
            var enfant = new CreateChildDto { LastName = "Axel", Gender = "m" };
            var responsable = new CreateGuardianDto { LastName = "Marine", Title = "mme" };

            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            postEnfant.EnsureSuccessStatusCode();
            postResponsable.EnsureSuccessStatusCode();

            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<ChildDto>();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdEnfant!.Id,
                GuardianId = createdResponsable!.Id,
                Relationship = "Tante"
            };

            var postLink = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            postLink.EnsureSuccessStatusCode();

            var updateDto = new UpdateLinkGuardianChildDto
            {
                ChildId = linkDto.ChildId,
                GuardianId = linkDto.GuardianId,
                Relationship = "Tuteur"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/linkresponsableenfant", updateDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task UpdateLinkResponsableEnfant_ShouldReturnNotFound_WhenLinkDoesNotExist()
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
            var response = await client.PutAsJsonAsync("/api/v1/linkresponsableenfant", updateDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteLinkResponsableEnfant_ShouldReturnNoContent_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateChildDto { LastName = "Lucas", Gender = "m" };
            var responsable = new CreateGuardianDto { LastName = "Claire", Title = "mme" };

            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            postEnfant.EnsureSuccessStatusCode();
            postResponsable.EnsureSuccessStatusCode();

            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<ChildDto>();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdEnfant!.Id,
                GuardianId = createdResponsable!.Id,
                Relationship = "Père"
            };

            var postLink = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            postLink.EnsureSuccessStatusCode();

            // Act
            var response = await client.DeleteAsync($"/api/v1/linkresponsableenfant/responsable/{createdResponsable.Id}/enfant/{createdEnfant.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteLinkResponsableEnfant_ShouldReturnNotFound_WhenLinkDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/linkresponsableenfant/responsable/9999/enfant/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}