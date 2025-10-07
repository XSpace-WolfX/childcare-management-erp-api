using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class DonneeSupplementaireApiTests
    {
        [Fact]
        public async Task GetAllDonneesSupplementaires_ShouldReturnDonneesSupplementaires_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/donneessupplementaires";
            var dto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync(url, dto);

            postReponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var donneesSupplementaires = await response.Content.ReadFromJsonAsync<List<AdditionalDataDto>>();
            donneesSupplementaires.ShouldNotBeNull();
            donneesSupplementaires.ShouldNotBeEmpty();
            donneesSupplementaires.ShouldAllBe(d => d.ParamName == dto.ParamName && d.ParamValue == dto.ParamValue);
        }

        [Fact]
        public async Task GetAllDonneesSupplementaires_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/donneessupplementaires";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var donneesSupplementaires = await response.Content.ReadFromJsonAsync<List<AdditionalDataDto>>();
            donneesSupplementaires.ShouldNotBeNull();
            donneesSupplementaires.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetDonneeSupplementaire_ShouldReturnDonneeSupplementaire_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", createDto);
            postReponse.EnsureSuccessStatusCode();

            var created = await postReponse.Content.ReadFromJsonAsync<AdditionalDataDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/donneessupplementaires/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var donneeSupplementaire = await response.Content.ReadFromJsonAsync<AdditionalDataDto>();
            donneeSupplementaire.ShouldNotBeNull();
            donneeSupplementaire.ParamName.ShouldBe(createDto.ParamName);
            donneeSupplementaire.ParamValue.ShouldBe(createDto.ParamValue);
        }

        [Fact]
        public async Task GetDonneeSupplementaire_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/donneessupplementaires/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateDonneeSupplementaire_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/donneessupplementaires";
            var dto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };

            // Act
            var response = await client.PostAsJsonAsync(url, dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var donneeSupplementaire = await response.Content.ReadFromJsonAsync<AdditionalDataDto>();
            donneeSupplementaire.ShouldNotBeNull();
            donneeSupplementaire.ParamName.ShouldBe(dto.ParamName);
            donneeSupplementaire.ParamValue.ShouldBe(dto.ParamValue);
        }

        [Fact]
        public async Task CreateDonneeSupplementaire_ShouldReturnBadRequest_WhenDtoIsNull()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            HttpContent nullBody = JsonContent.Create<object?>(null);

            // Act
            var response = await client.PostAsync("/api/v1/donneessupplementaires", nullBody);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaire_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", createDto);
            postReponse.EnsureSuccessStatusCode();
            var created = await postReponse.Content.ReadFromJsonAsync<AdditionalDataDto>();

            var updateDto = new UpdateAdditionalDataDto
            {
                Id = created!.Id,
                ParamName = "Parametre Modifié",
                ParamValue = "Valeur Modifiée"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/donneessupplementaires/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var donneeSupplementaire = await client.GetAsync($"/api/v1/donneessupplementaires/{created.Id}");
            var updated = await donneeSupplementaire.Content.ReadFromJsonAsync<AdditionalDataDto>();
            updated!.ParamName.ShouldBe(updateDto.ParamName);
            updated.ParamValue.ShouldBe(updateDto.ParamValue);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaire_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateAdditionalDataDto
            {
                Id = 2,
                ParamName = "Parametre Modifié",
                ParamValue = "Valeur Modifiée"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/donneessupplementaires/1", updateDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaire_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateAdditionalDataDto
            {
                Id = 9999,
                ParamName = "Parametre Modifié",
                ParamValue = "Valeur Modifiée"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/donneessupplementaires/9999", updateDto);

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteDonneeSupplementaire_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", createDto);
            postReponse.EnsureSuccessStatusCode();
            var created = await postReponse.Content.ReadFromJsonAsync<AdditionalDataDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/donneessupplementaires/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // Verify deletion
            var getResponse = await client.GetAsync($"/api/v1/donneessupplementaires/{created.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteDonneeSupplementaire_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/donneessupplementaires/9999");

            // Assert
            response.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }
    }
}