using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class PersonalSituationApiTests
    {
        [Fact]
        public async Task GetAllPersonalSituations_ShouldReturnPersonalSituations_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/personalsituations";
            var personalSituationDto = new CreatePersonalSituationDto
            {
                Regime = "Test Regime",
                MaritalStatus = "Description de la situation"
            };
            var postPersonalSituationResponse = await client.PostAsJsonAsync(url, personalSituationDto);
            postPersonalSituationResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personalSituations = await response.Content.ReadFromJsonAsync<List<PersonalSituationDto>>();
            personalSituations.ShouldNotBeNull();
            personalSituations.ShouldContain(ps => ps.MaritalStatus == "Description de la situation");
            personalSituations.ShouldContain(ps => ps.Regime == "Test Regime");
        }

        [Fact]
        public async Task GetAllPersonalSituation_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/personalsituations");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personalSituations = await response.Content.ReadFromJsonAsync<List<PersonalSituationDto>>();
            personalSituations.ShouldNotBeNull();
            personalSituations.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonalSituationById_ShouldReturnPersonalSituation_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var personalSituationDto = new CreatePersonalSituationDto
            {
                Regime = "Test Regime",
                MaritalStatus = "Description de la situation"
            };

            var postPersonalSituationResponse = await client.PostAsJsonAsync("/api/v1/personalsituations", personalSituationDto);
            postPersonalSituationResponse.EnsureSuccessStatusCode();

            var createdPersonalSituation = await postPersonalSituationResponse.Content.ReadFromJsonAsync<PersonalSituationDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/personalsituations/{createdPersonalSituation!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var personalSituation = await response.Content.ReadFromJsonAsync<PersonalSituationDto>();
            personalSituation.ShouldNotBeNull();
            personalSituation.Id.ShouldBe(createdPersonalSituation.Id);
            personalSituation.Regime.ShouldBe("Test Regime");
            personalSituation.MaritalStatus.ShouldBe("Description de la situation");
        }

        [Fact]
        public async Task GetPersonalSituationById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/personalsituations/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreatePersonalSituation_ShouldReturnCreated_WhenValidData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var personalSituationDto = new CreatePersonalSituationDto
            {
                Regime = "Test Regime",
                MaritalStatus = "Description de la situation"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/personalsituations", personalSituationDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var personalSituation = await response.Content.ReadFromJsonAsync<PersonalSituationDto>();
            personalSituation.ShouldNotBeNull();
            personalSituation.Regime.ShouldBe("Test Regime");
            personalSituation.MaritalStatus.ShouldBe("Description de la situation");
        }

        [Fact]
        public async Task UpdatePersonalSituation_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var personalSituationDto = new CreatePersonalSituationDto
            {
                Regime = "Initial Regime",
                MaritalStatus = "Initial Situation"
            };
            var postPersonalSituationResponse = await client.PostAsJsonAsync("/api/v1/personalsituations", personalSituationDto);
            postPersonalSituationResponse.EnsureSuccessStatusCode();
            var createdPersonalSituation = await postPersonalSituationResponse.Content.ReadFromJsonAsync<PersonalSituationDto>();

            var updateDto = new UpdatePersonalSituationDto
            {
                Id = createdPersonalSituation!.Id,
                Regime = "Updated Regime",
                MaritalStatus = "Updated Situation"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/personalsituations/{createdPersonalSituation.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/personalsituations/{createdPersonalSituation.Id}");
            var updatedSituation = await getResponse.Content.ReadFromJsonAsync<PersonalSituationDto>();
            updatedSituation.ShouldNotBeNull();
            updatedSituation.Id.ShouldBe(createdPersonalSituation.Id);
            updatedSituation.Regime.ShouldBe("Updated Regime");
            updatedSituation.MaritalStatus.ShouldBe("Updated Situation");
        }

        [Fact]
        public async Task UpdatePersonalSituation_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdatePersonalSituationDto
            {
                Id = 2,
                Regime = "Initial Regime",
                MaritalStatus = "Initial Situation"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/personalsituations/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de la situation personnelle ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdatePersonalSituation_ShouldReturnNotFound_WhenIdDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdatePersonalSituationDto
            {
                Id = 9999, // ID qui n'existe pas
                Regime = "Updated Regime",
                MaritalStatus = "Updated Situation"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/personalsituations/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task DeletePersonalSituation_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var personalSituationDto = new CreatePersonalSituationDto
            {
                Regime = "Test Regime",
                MaritalStatus = "Description de la situation"
            };
            var postPersonalSituationResponse = await client.PostAsJsonAsync("/api/v1/personalsituations", personalSituationDto);
            postPersonalSituationResponse.EnsureSuccessStatusCode();

            var createdPersonalSituation = await postPersonalSituationResponse.Content.ReadFromJsonAsync<PersonalSituationDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/personalsituations/{createdPersonalSituation!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/personalsituations/{createdPersonalSituation.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeletePersonalSituation_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/personalsituations/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");
        }
    }
}