using ChildcareManagementERP.Api.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ChildcareManagementERP.Api.IntegrationTests
{
    public class FinancialInformationApiTests
    {
        [Fact]
        public async Task GetAllFinancialInformations_ShouldReturnFinancialInformations_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/financialinformations";
            var dto = new CreateFinancialInformationDto
            {
                Model = "Test Modèle",
                AnnualIncome = (decimal?)1000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var financialInformations = await response.Content.ReadFromJsonAsync<List<FinancialInformationDto>>();
            financialInformations.ShouldNotBeNull();
            financialInformations.Count.ShouldBeGreaterThan(0);
            financialInformations.ShouldContain(inf => inf.Model == "Test Modèle");
        }

        [Fact]
        public async Task GetAllFinancialInformations_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/financialinformations";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var financialInformations = await response.Content.ReadFromJsonAsync<List<FinancialInformationDto>>();
            financialInformations.ShouldNotBeNull();
            financialInformations.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetFinancialInformationById_ShouldReturnFinancialInformation_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new CreateFinancialInformationDto
            {
                Model = "Test Modèle",
                AnnualIncome = (decimal?)1000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync("/api/v1/financialinformations", dto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<FinancialInformationDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/financialinformations/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var financialInformation = await response.Content.ReadFromJsonAsync<FinancialInformationDto>();
            financialInformation.ShouldNotBeNull();
            financialInformation.Id.ShouldBe(created.Id);
            financialInformation.Model.ShouldBe("Test Modèle");
        }

        [Fact]
        public async Task GetFinancialInformationById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/financialinformations/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateFinancialInformation_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new CreateFinancialInformationDto
            {
                Model = "Test Modèle",
                AnnualIncome = (decimal?)1000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/financialinformations", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<FinancialInformationDto>();
            created.ShouldNotBeNull();
            created.Model.ShouldBe("Test Modèle");
            created.AnnualIncome.ShouldBe(1000.00m);
        }

        [Fact]
        public async Task UpdateFinancialInformation_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateFinancialInformationDto
            {
                Model = "Test Modèle",
                AnnualIncome = (decimal?)1000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync("/api/v1/financialinformations", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<FinancialInformationDto>();

            var updateDto = new UpdateFinancialInformationDto
            {
                Id = created!.Id,
                Model = "Updated Modèle",
                AnnualIncome = (decimal?)2000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/financialinformations/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/financialinformations/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<FinancialInformationDto>();
            updated.ShouldNotBeNull();
            updated.Model.ShouldBe("Updated Modèle");
            updated.AnnualIncome.ShouldBe(2000.00m);
        }

        [Fact]
        public async Task UpdateFinancialInformation_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateFinancialInformationDto
            {
                Id = 2,
                Model = "Updated Modèle",
                AnnualIncome = (decimal?)2000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/financialinformations/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de l'information financière ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateFinancialInformation_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateFinancialInformationDto
            {
                Id = 9999,
                Model = "Updated Modèle",
                AnnualIncome = (decimal?)2000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/financialinformations/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task DeleteFinancialInformation_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateFinancialInformationDto
            {
                Model = "Test Modèle",
                AnnualIncome = (decimal?)1000.00,
                StartDate = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync("/api/v1/financialinformations", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<FinancialInformationDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/financialinformations/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/financialinformations/{created.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteFinancialInformation_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/financialinformations/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }
    }
}