using GestionAssociatifERP.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace GestionAssociatifERP.IntegrationTests
{
    public class EnfantApiTests
    {
        [Fact]
        public async Task GetAllEnfants_ShouldReturnEnfants_WhenDataExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/enfants";
            var dto = new CreateChildDto { LastName = "Alice" };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var enfants = await response.Content.ReadFromJsonAsync<List<ChildDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldContain(e => e.LastName == "Alice");
        }

        [Fact]
        public async Task GetAllEnfants_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/enfants";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var enfants = await response.Content.ReadFromJsonAsync<List<ChildDto>>();
            enfants.ShouldNotBeNull();
            enfants.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantById_ShouldReturnEnfant_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var enfant = await response.Content.ReadFromJsonAsync<ChildDto>();
            enfant.ShouldNotBeNull();
            enfant.Id.ShouldBe(created.Id);
            enfant.LastName.ShouldBe("Alice");
        }

        [Fact]
        public async Task GetEnfantById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithResponsables_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un enfant
            var enfantDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<ChildDto>();

            // 2. Créer un responsable
            var responsableDto = new CreateGuardianDto { LastName = "Dupont", Title = "m" };
            var postResponsable = await client.PostAsJsonAsync("/api/v1/responsables", responsableDto);
            postResponsable.EnsureSuccessStatusCode();
            var createdResponsable = await postResponsable.Content.ReadFromJsonAsync<GuardianDto>();

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
            var response = await client.GetAsync($"/api/v1/enfants/{createdEnfant.Id}/with-responsables");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithGuardiansDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdEnfant.Id);
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldContain(r => r.LastName == "Dupont");
        }

        [Fact]
        public async Task GetWithResponsables_ShouldReturnEmptyList_WhenNoResponsables()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Léo", Gender = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}/with-responsables");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithGuardiansDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetWithResponsables_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999/with-responsables");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithPersonnesAutorisees_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un enfant
            var enfantDto = new CreateChildDto { LastName = "Léo", Gender = "m" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var createdEnfant = await postEnfant.Content.ReadFromJsonAsync<ChildDto>();

            // 2. Créer une personne autorisée
            var personneDto = new CreateAuthorizedPersonDto { LastName = "Martin", FirstName = "Matin" };
            var postPers = await client.PostAsJsonAsync("/api/v1/personnesautorisees", personneDto);
            postPers.EnsureSuccessStatusCode();
            var createdPers = await postPers.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // 3. Lier les deux
            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdEnfant!.Id,
                AuthorizedPersonId = createdPers!.Id
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkpersonneautoriseeenfant", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{createdEnfant.Id}/with-personnes-autorisees");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAuthorizedPeopleDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdEnfant.Id);
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldContain(p => p.LastName == "Martin");
        }

        [Fact]
        public async Task GetWithPersonnesAutorisees_ShouldReturnEmptyList_WhenNoPersonnesAutorisees()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfantDto = new CreateChildDto { LastName = "Sasha", Gender = "non_specifie" };
            var post = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}/with-personnes-autorisees");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAuthorizedPeopleDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetWithPersonnesAutorisees_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999/with-personnes-autorisees");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetWithDonneesSupplementaires_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Créer un enfant
            var enfantDto = new CreateChildDto { LastName = "Jules", Gender = "m" };
            var postEnfant = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            postEnfant.EnsureSuccessStatusCode();
            var created = await postEnfant.Content.ReadFromJsonAsync<ChildDto>();

            // Créer une donnée supplémentaire liée à l’enfant
            var dsDto = new CreateAdditionalDataDto
            {
                ChildId = created!.Id,
                ParamName = "Allergie",
                ParamValue = "Gluten",
                ParamType = "Texte",
                Comment = "Important"
            };

            var postDs = await client.PostAsJsonAsync("/api/v1/donneessupplementaires", dsDto);
            postDs.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created.Id}/with-donnees-supplementaires");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAdditionalDatasDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldContain(d => d.ParamName == "Allergie");
        }

        [Fact]
        public async Task GetWithDonneesSupplementaires_ShouldReturnEmptyList_WhenNoDonneesSupplementaires()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var enfantDto = new CreateChildDto { LastName = "Sami", Gender = "autre" };
            var post = await client.PostAsJsonAsync("/api/v1/enfants", enfantDto);
            post.EnsureSuccessStatusCode();
            var created = await post.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/enfants/{created!.Id}/with-donnees-supplementaires");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAdditionalDatasDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetWithDonneesSupplementaires_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/enfants/9999/with-donnees-supplementaires");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateEnfant_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var dto = new CreateChildDto
            {
                Gender = "mme",
                LastName = "Camille",
                FirstName = "Dupont"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/enfants", dto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<ChildDto>();
            created.ShouldNotBeNull();
            created.LastName.ShouldBe("Camille");
            created.Gender.ShouldBe("mme");
        }

        [Fact]
        public async Task UpdateEnfant_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Emma", Gender = "mme" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            var updateDto = new UpdateChildDto
            {
                Id = created!.Id,
                LastName = "Emma Modifiée",
                Gender = "mme"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/enfants/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/enfants/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<ChildDto>();
            updated.ShouldNotBeNull();
            updated!.LastName.ShouldBe("Emma Modifiée");
        }

        [Fact]
        public async Task UpdateEnfant_ShouldReturnBadRequest_WhenIdMismatch()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateChildDto
            {
                Id = 2,
                LastName = "Invalid",
                Gender = "m"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/enfants/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateEnfant_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var updateDto = new UpdateChildDto
            {
                Id = 9999,
                LastName = "Inconnu",
                Gender = "autre"
            };

            // Act
            var response = await client.PutAsJsonAsync("/api/v1/enfants/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task DeleteEnfant_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Lucas", Gender = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/enfants", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/enfants/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/enfants/{created.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteEnfant_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/v1/enfants/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }
    }
}