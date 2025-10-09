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
            var dto = new CreateGuardianDto { LastName = "Alice" };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var responsables = await response.Content.ReadFromJsonAsync<List<GuardianDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldContain(e => e.LastName == "Alice");
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

            var responsables = await response.Content.ReadFromJsonAsync<List<GuardianDto>>();
            responsables.ShouldNotBeNull();
            responsables.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetResponsableById_ShouldReturnResponsable_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateGuardianDto { LastName = "Bob" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/responsables", createDto);
            postResponse.EnsureSuccessStatusCode();

            var createdResponsable = await postResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var responsable = await response.Content.ReadFromJsonAsync<GuardianDto>();
            responsable.ShouldNotBeNull();
            responsable.Id.ShouldBe(createdResponsable.Id);
            responsable.LastName.ShouldBe("Bob");
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
            var responsableDto = new CreateGuardianDto { LastName = "Dupont", Title = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Créer un enfant
            var enfantDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<ChildDto>();

            // 3. Lier responsable ↔ enfant
            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdEnfant!.Id,
                GuardianId = createdResponsable!.Id,
                Relationship = "Parent"
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkresponsableenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithChildrenDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.Children.ShouldNotBeNull();
            result.Children.ShouldContain(r => r.LastName == "Alice");
        }

        [Fact]
        public async Task GetWithEnfants_ShouldReturnEmptyList_WhenNoEnfants()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsableDto = new CreateGuardianDto { LastName = "Martin", Title = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();

            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-enfants");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithChildrenDto>();
            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
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
            var responsableDto = new CreateGuardianDto { LastName = "Lefevre", Title = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Créer une information financière
            var infoFinanciereDto = new CreateFinancialInformationDto
            {
                GuardianId = createdResponsable!.Id,
                FamilyQuotient = 100,
                StartDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            };
            var postInfoFinanciere = await client.PostAsJsonAsync("/api/v1/informationsfinancieres", infoFinanciereDto);
            postInfoFinanciere.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-information-financiere");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithFinancialInformationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.FinancialInformation.ShouldNotBeNull();
            result.FinancialInformation.FamilyQuotient.ShouldBe(100);
        }

        [Fact]
        public async Task GetWithInformationFinanciere_ShouldReturnEmpty_WhenNoInformationFinanciere()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsableDto = new CreateGuardianDto { LastName = "Durand", Title = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-information-financiere");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithFinancialInformationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.FinancialInformation.ShouldBeNull();
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
            var responsableDto = new CreateGuardianDto { LastName = "Bernard", Title = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Créer une situation personnelle
            var situationPersonnelleDto = new CreatePersonalSituationDto
            {
                GuardianId = createdResponsable!.Id,
                FamilySituation = "Célibataire",
                Sector = "Test"
            };
            var postSituationPersonnelle = await client.PostAsJsonAsync("/api/v1/situationspersonnelles", situationPersonnelleDto);
            postSituationPersonnelle.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-situation-personnelle");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithPersonalSituationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.PersonalSituation.ShouldNotBeNull();
            result.PersonalSituation.FamilySituation.ShouldBe("Célibataire");
        }

        [Fact]
        public async Task GetWithSituationPersonnelle_ShouldReturnEmpty_WhenNoSituationPersonnelle()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var responsableDto = new CreateGuardianDto { LastName = "Lemoine", Title = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();

            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/responsables/{createdResponsable!.Id}/with-situation-personnelle");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithPersonalSituationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdResponsable.Id);
            result.PersonalSituation.ShouldBeNull();
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

            var createDto = new CreateGuardianDto { LastName = "Martin", Title = "m" };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/responsables", createDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var createdResponsable = await response.Content.ReadFromJsonAsync<GuardianDto>();
            createdResponsable.ShouldNotBeNull();
            createdResponsable.LastName.ShouldBe("Martin");
            createdResponsable.Title.ShouldBe("m");
        }

        [Fact]
        public async Task UpdateResponsable_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var createDto = new CreateGuardianDto { LastName = "Dupuis", Title = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/responsables", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdResponsable = await postResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Mettre à jour le responsable
            var updateDto = new UpdateGuardianDto
            {
                Id = createdResponsable!.Id,
                LastName = "Dupuis Modifié",
                Title = "mme"
            };

            // Act
            var updateResponse = await client.PutAsJsonAsync($"/api/v1/responsables/{createdResponsable.Id}", updateDto);

            // Assert
            updateResponse.EnsureSuccessStatusCode();
            updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/responsables/{createdResponsable.Id}");
            var updatedResponsable = await getResponse.Content.ReadFromJsonAsync<GuardianDto>();
            updatedResponsable.ShouldNotBeNull();
            updatedResponsable.LastName.ShouldBe("Dupuis Modifié");
        }

        [Fact]
        public async Task UpdateResponsable_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateGuardianDto
            {
                Id = 2,
                LastName = "Lemoine Modifié",
                Title = "mme"
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

            var updateDto = new UpdateGuardianDto
            {
                Id = 9999,
                LastName = "Inconnu",
                Title = "m"
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

            var createDto = new CreateGuardianDto { LastName = "Garnier", Title = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/responsables", createDto);
            postResponse.EnsureSuccessStatusCode();
            var createdResponsable = await postResponse.Content.ReadFromJsonAsync<GuardianDto>();

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