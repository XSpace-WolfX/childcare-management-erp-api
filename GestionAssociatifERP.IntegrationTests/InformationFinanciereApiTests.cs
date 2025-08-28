using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class InformationFinanciereApiTests
    {
        [Fact]
        public async Task GetAllInformationFinanciere_ShouldReturnInformationsFinancieres_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/InformationsFinancieres";
            var dto = new CreateInformationFinanciereDto
            {
                Modele = "Test Modèle",
                RevenuAnnuel = (decimal?)1000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var informationsFinancieres = await response.Content.ReadFromJsonAsync<List<InformationFinanciereDto>>();
            informationsFinancieres.ShouldNotBeNull();
            informationsFinancieres.Count.ShouldBeGreaterThan(0);
            informationsFinancieres.ShouldContain(inf => inf.Modele == "Test Modèle");
        }

        [Fact]
        public async Task GetAllInformationFinanciere_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/InformationsFinancieres";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var informationsFinancieres = await response.Content.ReadFromJsonAsync<List<InformationFinanciereDto>>();
            informationsFinancieres.ShouldNotBeNull();
            informationsFinancieres.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetInformationFinanciereById_ShouldReturnInformationFinanciere_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new CreateInformationFinanciereDto
            {
                Modele = "Test Modèle",
                RevenuAnnuel = (decimal?)1000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync("/api/v1/informationsfinancieres", dto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<InformationFinanciereDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/informationsfinancieres/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var informationFinanciere = await response.Content.ReadFromJsonAsync<InformationFinanciereDto>();
            informationFinanciere.ShouldNotBeNull();
            informationFinanciere.Id.ShouldBe(created.Id);
            informationFinanciere.Modele.ShouldBe("Test Modèle");
        }

        [Fact]
        public async Task GetInformationFinanciereById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/informationsfinancieres/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateInformationFinanciere_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new CreateInformationFinanciereDto
            {
                Modele = "Test Modèle",
                RevenuAnnuel = (decimal?)1000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now)
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/informationsfinancieres", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<InformationFinanciereDto>();
            created.ShouldNotBeNull();
            created.Modele.ShouldBe("Test Modèle");
            created.RevenuAnnuel.ShouldBe(1000.00m);
        }

        [Fact]
        public async Task UpdateInformationFinanciere_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateInformationFinanciereDto
            {
                Modele = "Test Modèle",
                RevenuAnnuel = (decimal?)1000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync("/api/v1/informationsfinancieres", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<InformationFinanciereDto>();

            var updateDto = new UpdateInformationFinanciereDto
            {
                Id = created!.Id,
                Modele = "Updated Modèle",
                RevenuAnnuel = (decimal?)2000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/informationsfinancieres/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/informationsfinancieres/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<InformationFinanciereDto>();
            updated.ShouldNotBeNull();
            updated.Modele.ShouldBe("Updated Modèle");
            updated.RevenuAnnuel.ShouldBe(2000.00m);
        }

        [Fact]
        public async Task UpdateInformationFinanciere_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateInformationFinanciereDto
            {
                Id = 2,
                Modele = "Updated Modèle",
                RevenuAnnuel = (decimal?)2000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/informationsfinancieres/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de l'information financière ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateInformationFinanciere_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateInformationFinanciereDto
            {
                Id = 9999,
                Modele = "Updated Modèle",
                RevenuAnnuel = (decimal?)2000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now.AddDays(1))
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/informationsfinancieres/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task DeleteInformationFinanciere_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateInformationFinanciereDto
            {
                Modele = "Test Modèle",
                RevenuAnnuel = (decimal?)1000.00,
                DateDebut = DateOnly.FromDateTime(DateTime.Now)
            };
            var postResponse = await client.PostAsJsonAsync("/api/v1/informationsfinancieres", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<InformationFinanciereDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/informationsfinancieres/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/informationsfinancieres/{created.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteInformationFinanciere_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/informationsfinancieres/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }
    }
}