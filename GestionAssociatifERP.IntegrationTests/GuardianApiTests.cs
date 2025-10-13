using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class GuardianApiTests
    {
        [Fact]
        public async Task GetAllGuardians_ShouldReturnGuardians_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/guardians";
            var guardianDto = new CreateGuardianDto { LastName = "Alice" };
            var postGuardianResponse = await client.PostAsJsonAsync(url, guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var guardians = await response.Content.ReadFromJsonAsync<List<GuardianDto>>();
            guardians.ShouldNotBeNull();
            guardians.ShouldContain(g => g.LastName == "Alice");
        }

        [Fact]
        public async Task GetAllGuardians_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/guardians";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var guardians = await response.Content.ReadFromJsonAsync<List<GuardianDto>>();
            guardians.ShouldNotBeNull();
            guardians.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetGuardianById_ShouldReturnGuardian_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var guardianDto = new CreateGuardianDto { LastName = "Bob" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/guardians/{createdGuardian!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var guardian = await response.Content.ReadFromJsonAsync<GuardianDto>();
            guardian.ShouldNotBeNull();
            guardian.Id.ShouldBe(createdGuardian.Id);
            guardian.LastName.ShouldBe("Bob");
        }

        [Fact]
        public async Task GetGuardianById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/guardians/999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetGuardianWithChildren__ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var guardianDto = new CreateGuardianDto { LastName = "Dupont", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Créer un enfant
            var childDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // 3. Lier responsable ↔ enfant
            var linkDto = new CreateLinkGuardianChildDto
            {
                ChildId = createdChild!.Id,
                GuardianId = createdGuardian!.Id,
                Relationship = "Parent"
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkguardianchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/guardians/{createdGuardian!.Id}/with-children");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithChildrenDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdGuardian.Id);
            result.Children.ShouldNotBeNull();
            result.Children.ShouldContain(c => c.LastName == "Alice");
        }

        [Fact]
        public async Task GetGuardianWithChildren_ShouldReturnEmptyList_WhenNoChild()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var GuardianDto = new CreateGuardianDto { LastName = "Martin", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", GuardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/guardians/{createdGuardian!.Id}/with-children");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithChildrenDto>();
            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdGuardian.Id);
            result.Children.ShouldNotBeNull();
            result.Children.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetGuardianWithChildren_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/guardians/9999/with-children");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetGuardianWithFinancialInformation_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var guardianDto = new CreateGuardianDto { LastName = "Lefevre", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Créer une information financière
            var financialInformationDto = new CreateFinancialInformationDto
            {
                GuardianId = createdGuardian!.Id,
                FamilyQuotient = 100,
                StartDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day)
            };
            var postFinancialInformationResponse = await client.PostAsJsonAsync("/api/v1/financialinformations", financialInformationDto);
            postFinancialInformationResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/guardians/{createdGuardian!.Id}/with-financial-information");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithFinancialInformationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdGuardian.Id);
            result.FinancialInformation.ShouldNotBeNull();
            result.FinancialInformation.FamilyQuotient.ShouldBe(100);
        }

        [Fact]
        public async Task GetGuardianWithFinancialInformation_ShouldReturnEmpty_WhenNoFinancialInformation()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var guardianDto = new CreateGuardianDto { LastName = "Durand", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/guardians/{createdGuardian!.Id}/with-financial-information");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithFinancialInformationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdGuardian.Id);
            result.FinancialInformation.ShouldBeNull();
        }

        [Fact]
        public async Task GetGuardianWithFinancialInformation_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/guardians/9999/with-financial-information");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetGuardianWithPersonalSituation_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var guardianDto = new CreateGuardianDto { LastName = "Bernard", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Créer une situation personnelle
            var personalSituationDto = new CreatePersonalSituationDto
            {
                GuardianId = createdGuardian!.Id,
                MaritalStatus = "Célibataire",
                Sector = "Test"
            };
            var postPersonalSituationResponse = await client.PostAsJsonAsync("/api/v1/personalsituations", personalSituationDto);
            postPersonalSituationResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/guardians/{createdGuardian!.Id}/with-personal-situation");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithPersonalSituationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdGuardian.Id);
            result.PersonalSituation.ShouldNotBeNull();
            result.PersonalSituation.MaritalStatus.ShouldBe("Célibataire");
        }

        [Fact]
        public async Task GetGuardianWithPersonalSituation_ShouldReturnEmpty_WhenNoPersonalSituation()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var guardianDto = new CreateGuardianDto { LastName = "Lemoine", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();

            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/guardians/{createdGuardian!.Id}/with-personal-situation");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<GuardianWithPersonalSituationDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdGuardian.Id);
            result.PersonalSituation.ShouldBeNull();
        }

        [Fact]
        public async Task GetGuardianWithPersonalSituation_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/guardians/9999/with-personal-situation");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateGuardian_ShouldReturnCreated_WhenValidData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var guardianDto = new CreateGuardianDto { LastName = "Martin", Title = "m" };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var guardian = await response.Content.ReadFromJsonAsync<GuardianDto>();
            guardian.ShouldNotBeNull();
            guardian.LastName.ShouldBe("Martin");
            guardian.Title.ShouldBe("m");
        }

        [Fact]
        public async Task UpdateGuardian_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un responsable
            var guardianDto = new CreateGuardianDto { LastName = "Dupuis", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // 2. Mettre à jour le responsable
            var updateDto = new UpdateGuardianDto
            {
                Id = createdGuardian!.Id,
                LastName = "Dupuis Modifié",
                Title = "mme"
            };

            // Act
            var updateResponse = await client.PutAsJsonAsync($"/api/v1/guardians/{createdGuardian.Id}", updateDto);

            // Assert
            updateResponse.EnsureSuccessStatusCode();
            updateResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/guardians/{createdGuardian.Id}");
            var updatedResponsable = await getResponse.Content.ReadFromJsonAsync<GuardianDto>();
            updatedResponsable.ShouldNotBeNull();
            updatedResponsable.LastName.ShouldBe("Dupuis Modifié");
        }

        [Fact]
        public async Task UpdateGuardian_ShouldReturnBadRequest_WhenIdMismatch()
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
            var updateResponse = await client.PutAsJsonAsync($"/api/v1/guardians/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(updateResponse, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant du responsable ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateGuardian_ShouldReturnNotFound_WhenDoesNotExist()
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
            var updateResponse = await client.PutAsJsonAsync($"/api/v1/guardians/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(updateResponse, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task DeleteGuardian_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var guardianDto = new CreateGuardianDto { LastName = "Garnier", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

            // Act
            var deleteResponse = await client.DeleteAsync($"/api/v1/guardians/{createdGuardian!.Id}");

            // Assert
            deleteResponse.EnsureSuccessStatusCode();
            deleteResponse.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/guardians/{createdGuardian.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteGuardian_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var deleteResponse = await client.DeleteAsync("/api/v1/guardians/9999");
            var exception = await AssertProblemDetails.AssertProblem(deleteResponse, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }
    }
}