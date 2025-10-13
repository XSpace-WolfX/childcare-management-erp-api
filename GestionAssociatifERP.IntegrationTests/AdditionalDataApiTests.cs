using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class AdditionalDataApiTests
    {
        [Fact]
        public async Task GetAllAdditionalDatas_ShouldReturnAdditionalDatas_WhenDatasExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/additionaldatas";
            var dto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync(url, dto);

            postReponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var additionalDatas = await response.Content.ReadFromJsonAsync<List<AdditionalDataDto>>();
            additionalDatas.ShouldNotBeNull();
            additionalDatas.ShouldNotBeEmpty();
            additionalDatas.ShouldAllBe(d => d.ParamName == dto.ParamName && d.ParamValue == dto.ParamValue);
        }

        [Fact]
        public async Task GetAllAdditionalDatas_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/additionaldatas";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var additionalDatas = await response.Content.ReadFromJsonAsync<List<AdditionalDataDto>>();
            additionalDatas.ShouldNotBeNull();
            additionalDatas.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAdditionalData_ShouldReturnAdditionalData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/additionaldatas", createDto);
            postReponse.EnsureSuccessStatusCode();

            var created = await postReponse.Content.ReadFromJsonAsync<AdditionalDataDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/additionaldatas/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var additionalData = await response.Content.ReadFromJsonAsync<AdditionalDataDto>();
            additionalData.ShouldNotBeNull();
            additionalData.ParamName.ShouldBe(createDto.ParamName);
            additionalData.ParamValue.ShouldBe(createDto.ParamValue);
        }

        [Fact]
        public async Task GetAdditionalData_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/additionaldatas/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateAdditionalData_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/additionaldatas";
            var dto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };

            // Act
            var response = await client.PostAsJsonAsync(url, dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var additionalData = await response.Content.ReadFromJsonAsync<AdditionalDataDto>();
            additionalData.ShouldNotBeNull();
            additionalData.ParamName.ShouldBe(dto.ParamName);
            additionalData.ParamValue.ShouldBe(dto.ParamValue);
        }

        [Fact]
        public async Task UpdateAdditionalData_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/additionaldatas", createDto);
            postReponse.EnsureSuccessStatusCode();
            var created = await postReponse.Content.ReadFromJsonAsync<AdditionalDataDto>();

            var updateDto = new UpdateAdditionalDataDto
            {
                Id = created!.Id,
                ParamName = "Parametre Modifié",
                ParamValue = "Valeur Modifiée"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/additionaldatas/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var additionalData = await client.GetAsync($"/api/v1/additionaldatas/{created.Id}");
            var updated = await additionalData.Content.ReadFromJsonAsync<AdditionalDataDto>();
            updated!.ParamName.ShouldBe(updateDto.ParamName);
            updated.ParamValue.ShouldBe(updateDto.ParamValue);
        }

        [Fact]
        public async Task UpdateAdditionalData_ShouldReturnBadRequest_WhenIdMismatch()
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
            var response = await client.PutAsJsonAsync($"/api/v1/additionaldatas/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de la donnée supplémentaire ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateAdditionalData_ShouldReturnNotFound_WhenDoesNotExist()
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
            var response = await client.PutAsJsonAsync($"/api/v1/additionaldatas/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task DeleteAdditionalData_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateAdditionalDataDto { ParamName = "Parametre Test", ParamValue = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/additionaldatas", createDto);
            postReponse.EnsureSuccessStatusCode();
            var created = await postReponse.Content.ReadFromJsonAsync<AdditionalDataDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/additionaldatas/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            // Verify deletion
            var getResponse = await client.GetAsync($"/api/v1/additionaldatas/{created.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteAdditionalData_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync("/api/v1/additionaldatas/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }
    }
}