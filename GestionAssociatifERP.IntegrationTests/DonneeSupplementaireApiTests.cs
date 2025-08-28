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
            var dto = new CreateDonneeSupplementaireDto { Parametre = "Parametre Test", Valeur = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync(url, dto);

            postReponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var donneesSupplementaires = await response.Content.ReadFromJsonAsync<List<DonneeSupplementaireDto>>();
            donneesSupplementaires.ShouldNotBeNull();
            donneesSupplementaires.ShouldNotBeEmpty();
            donneesSupplementaires.ShouldAllBe(d => d.Parametre == dto.Parametre && d.Valeur == dto.Valeur);
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

            var donneesSupplementaires = await response.Content.ReadFromJsonAsync<List<DonneeSupplementaireDto>>();
            donneesSupplementaires.ShouldNotBeNull();
            donneesSupplementaires.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetDonneeSupplementaire_ShouldReturnDonneeSupplementaire_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateDonneeSupplementaireDto { Parametre = "Parametre Test", Valeur = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", createDto);
            postReponse.EnsureSuccessStatusCode();

            var created = await postReponse.Content.ReadFromJsonAsync<DonneeSupplementaireDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/donneessupplementaires/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var donneeSupplementaire = await response.Content.ReadFromJsonAsync<DonneeSupplementaireDto>();
            donneeSupplementaire.ShouldNotBeNull();
            donneeSupplementaire.Parametre.ShouldBe(createDto.Parametre);
            donneeSupplementaire.Valeur.ShouldBe(createDto.Valeur);
        }

        [Fact]
        public async Task GetDonneeSupplementaire_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/donneessupplementaires/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateDonneeSupplementaire_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/donneessupplementaires";
            var dto = new CreateDonneeSupplementaireDto { Parametre = "Parametre Test", Valeur = "Valeur Test" };

            // Act
            var response = await client.PostAsJsonAsync(url, dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var donneeSupplementaire = await response.Content.ReadFromJsonAsync<DonneeSupplementaireDto>();
            donneeSupplementaire.ShouldNotBeNull();
            donneeSupplementaire.Parametre.ShouldBe(dto.Parametre);
            donneeSupplementaire.Valeur.ShouldBe(dto.Valeur);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaire_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateDonneeSupplementaireDto { Parametre = "Parametre Test", Valeur = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", createDto);
            postReponse.EnsureSuccessStatusCode();
            var created = await postReponse.Content.ReadFromJsonAsync<DonneeSupplementaireDto>();

            var updateDto = new UpdateDonneeSupplementaireDto
            {
                Id = created!.Id,
                Parametre = "Parametre Modifié",
                Valeur = "Valeur Modifiée"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/donneessupplementaires/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var donneeSupplementaire = await client.GetAsync($"/api/v1/donneessupplementaires/{created.Id}");
            var updated = await donneeSupplementaire.Content.ReadFromJsonAsync<DonneeSupplementaireDto>();
            updated!.Parametre.ShouldBe(updateDto.Parametre);
            updated.Valeur.ShouldBe(updateDto.Valeur);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaire_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateDonneeSupplementaireDto
            {
                Id = 2,
                Parametre = "Parametre Modifié",
                Valeur = "Valeur Modifiée"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/donneessupplementaires/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de la donnée supplémentaire ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateDonneeSupplementaire_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateDonneeSupplementaireDto
            {
                Id = 9999,
                Parametre = "Parametre Modifié",
                Valeur = "Valeur Modifiée"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/donneessupplementaires/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task DeleteDonneeSupplementaire_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateDonneeSupplementaireDto { Parametre = "Parametre Test", Valeur = "Valeur Test" };
            var postReponse = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", createDto);
            postReponse.EnsureSuccessStatusCode();
            var created = await postReponse.Content.ReadFromJsonAsync<DonneeSupplementaireDto>();

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
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }
    }
}