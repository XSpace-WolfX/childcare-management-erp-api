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

            var enfant = new CreateEnfantDto { Nom = "Lina", Civilite = "mme" };
            var responsable = new CreateResponsableDto { Nom = "Sophie", Civilite = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = enfantCreated!.Id,
                ResponsableId = responsableCreated!.Id,
                Affiliation = "Mère"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/enfant/{enfantCreated.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var responsables = await response.Content.ReadFromJsonAsync<List<LinkResponsableEnfantDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldContain(r => r.ResponsableId == responsableCreated.Id);
        }

        [Fact]
        public async Task GetResponsablesByEnfantId_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateEnfantDto { Nom = "Noa", Civilite = "non_specifie" };
            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            enfantResponse.EnsureSuccessStatusCode();
            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/enfant/{enfantCreated!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var responsables = await response.Content.ReadFromJsonAsync<List<LinkResponsableEnfantDto>>();
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
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetEnfantsByResponsableId_ShouldReturnList_WhenLinksExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateEnfantDto { Nom = "Émile", Civilite = "m" };
            var responsable = new CreateResponsableDto { Nom = "Nathalie", Civilite = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = enfantCreated!.Id,
                ResponsableId = responsableCreated!.Id,
                Affiliation = "Tante"
            };

            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/responsable/{responsableCreated.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var enfants = await response.Content.ReadFromJsonAsync<List<LinkResponsableEnfantDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldContain(e => e.EnfantId == enfantCreated.Id);
        }

        [Fact]
        public async Task GetEnfantsByResponsableId_ShouldReturnEmptyList_WhenNoLinks()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsable = new CreateResponsableDto { Nom = "Julie", Civilite = "mme" };
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            responsableResponse.EnsureSuccessStatusCode();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/linkresponsableenfant/responsable/{responsableCreated!.Id}");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var enfants = await response.Content.ReadFromJsonAsync<List<LinkResponsableEnfantDto>>();
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
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task ExistsLinkResponsableEnfant_ShouldReturnTrue_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateEnfantDto { Nom = "Élise", Civilite = "mme" };
            var responsable = new CreateResponsableDto { Nom = "Paul", Civilite = "m" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = enfantCreated!.Id,
                ResponsableId = responsableCreated!.Id,
                Affiliation = "Mère"
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

            var enfant = new CreateEnfantDto { Nom = "Hugo", Civilite = "m" };
            var responsable = new CreateResponsableDto { Nom = "Claire", Civilite = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

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

            var enfant = new CreateEnfantDto { Nom = "Camille", Civilite = "mme" };
            var responsable = new CreateResponsableDto { Nom = "Jean", Civilite = "m" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            var dto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = enfantCreated!.Id,
                ResponsableId = responsableCreated!.Id,
                Affiliation = "Père"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", dto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.OK);
            var result = await response.Content.ReadFromJsonAsync<LinkResponsableEnfantDto>();
            result.ShouldNotBeNull();
            result.EnfantId.ShouldBe(dto.EnfantId);
            result.ResponsableId.ShouldBe(dto.ResponsableId);
            result.Affiliation.ShouldBe("Père");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfant_ShouldReturnNotFound_WhenEnfantDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Étape 1 – Créer un responsable valide
            var createResponsableDto = new CreateResponsableDto
            {
                Nom = "Test",
                Prenom = "Responsable",
                Email = "test.responsable@example.com",
                Telephone = "0600000001"
            };

            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", createResponsableDto);
            responsableResponse.EnsureSuccessStatusCode();
            var responsable = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            // Étape 2 – EnfantId inexistant
            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = 9999,
                ResponsableId = responsable!.Id,
                Affiliation = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfant_ShouldReturnNotFound_WhenResponsableDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Étape 1 – Créer un enfant valide
            var createEnfantDto = new CreateEnfantDto
            {
                Nom = "Test",
                Prenom = "Enfant",
                DateNaissance = DateOnly.FromDateTime(DateTime.Today.AddYears(-6)),
                Civilite = "F"
            };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", createEnfantDto);
            enfantResponse.EnsureSuccessStatusCode();
            var enfant = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();

            // Étape 2 – ResponsableId inexistant
            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = enfant!.Id,
                ResponsableId = 9999,
                Affiliation = "TestAffiliation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfant_ShouldReturnConflict_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateEnfantDto { Nom = "Julien", Civilite = "m" };
            var responsable = new CreateResponsableDto { Nom = "Lucie", Civilite = "mme" };

            var enfantResponse = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var responsableResponse = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            enfantResponse.EnsureSuccessStatusCode();
            responsableResponse.EnsureSuccessStatusCode();

            var enfantCreated = await enfantResponse.Content.ReadFromJsonAsync<EnfantDto>();
            var responsableCreated = await responsableResponse.Content.ReadFromJsonAsync<ResponsableDto>();

            var dto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = enfantCreated!.Id,
                ResponsableId = responsableCreated!.Id,
                Affiliation = "Mère"
            };

            // First creation
            var firstResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", dto);
            firstResponse.StatusCode.ShouldBe(HttpStatusCode.OK);

            // Act - Second creation should fail
            var secondResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", dto);
            var exception = await AssertProblemDetails.AssertProblem(secondResponse, HttpStatusCode.Conflict);

            // Assert
            exception.Detail.ShouldBe("Ce lien existe déjà entre ce responsable et cet enfant.");
        }

        [Fact]
        public async Task UpdateLinkResponsableEnfant_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();
            
            var enfant = new CreateEnfantDto { Nom = "Axel", Civilite = "m" };
            var responsable = new CreateResponsableDto { Nom = "Marine", Civilite = "mme" };

            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            postEnfant.EnsureSuccessStatusCode();
            postResponsable.EnsureSuccessStatusCode();

            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<EnfantDto>();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = createdEnfant!.Id,
                ResponsableId = createdResponsable!.Id,
                Affiliation = "Tante"
            };

            var postLink = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            postLink.EnsureSuccessStatusCode();

            var updateDto = new UpdateLinkResponsableEnfantDto
            {
                EnfantId = linkDto.EnfantId,
                ResponsableId = linkDto.ResponsableId,
                Affiliation = "Tuteur"
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

            var updateDto = new UpdateLinkResponsableEnfantDto
            {
                EnfantId = 999,
                ResponsableId = 888,
                Affiliation = "Tuteur"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/linkresponsableenfant", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun lien Responsable / Enfant trouvé à mettre à jour.");
        }

        [Fact]
        public async Task DeleteLinkResponsableEnfant_ShouldReturnNoContent_WhenLinkExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfant = new CreateEnfantDto { Nom = "Lucas", Civilite = "m" };
            var responsable = new CreateResponsableDto { Nom = "Claire", Civilite = "mme" };

            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfant);
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsable);
            postEnfant.EnsureSuccessStatusCode();
            postResponsable.EnsureSuccessStatusCode();

            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<EnfantDto>();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<ResponsableDto>();

            var linkDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = createdEnfant!.Id,
                ResponsableId = createdResponsable!.Id,
                Affiliation = "Père"
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
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun lien Responsable / Enfant trouvé à supprimer.");
        }
    }
}