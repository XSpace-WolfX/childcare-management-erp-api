using ChildcareManagementERP.Api.Dtos.V1;
using Shouldly;
using System.Net;
using System.Net.Http.Json;

namespace ChildcareManagementERP.Api.IntegrationTests
{
    public class ChildApiTests
    {
        [Fact]
        public async Task GetAllChildren_ShouldReturnChildren_WhenDatasExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/children";
            var dto = new CreateChildDto { LastName = "Alice" };
            var postResponse = await client.PostAsJsonAsync(url, dto);
            postResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var children = await response.Content.ReadFromJsonAsync<List<ChildDto>>();
            children.ShouldNotBeNull();
            children.ShouldContain(e => e.LastName == "Alice");
        }

        [Fact]
        public async Task GetAllChildren_ShouldReturnOkAndEmptyList_WhenNoData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var url = "/api/v1/children";

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var children = await response.Content.ReadFromJsonAsync<List<ChildDto>>();
            children.ShouldNotBeNull();
            children.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildById_ShouldReturnChild_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/children", createDto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/children/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var child = await response.Content.ReadFromJsonAsync<ChildDto>();
            child.ShouldNotBeNull();
            child.Id.ShouldBe(created.Id);
            child.LastName.ShouldBe("Alice");
        }

        [Fact]
        public async Task GetChildById_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/children/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetChildWithGuardians_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un enfant
            var childDto = new CreateChildDto { LastName = "Alice", Gender = "mme" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // 2. Créer un responsable
            var guardianDto = new CreateGuardianDto { LastName = "Dupont", Title = "m" };
            var postGuardianResponse = await client.PostAsJsonAsync("/api/v1/guardians", guardianDto);
            postGuardianResponse.EnsureSuccessStatusCode();
            var createdGuardian = await postGuardianResponse.Content.ReadFromJsonAsync<GuardianDto>();

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
            var response = await client.GetAsync($"/api/v1/children/{createdChild.Id}/with-guardians");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithGuardiansDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdChild.Id);
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldContain(r => r.LastName == "Dupont");
        }

        [Fact]
        public async Task GetChildWithGuardians_ShouldReturnEmptyList_WhenNoGuardian()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Léo", Gender = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/children", createDto);
            postResponse.EnsureSuccessStatusCode();

            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/children/{created!.Id}/with-guardians");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithGuardiansDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(created.Id);
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildWithGuardians_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/children/9999/with-guardians");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetChildWithAuthorizedPeople_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // 1. Créer un enfant
            var childDto = new CreateChildDto { LastName = "Léo", Gender = "m" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // 2. Créer une personne autorisée
            var authorizedPersonDto = new CreateAuthorizedPersonDto { LastName = "Martin", FirstName = "Matin" };
            var postAuthorizedPersonResponse = await client.PostAsJsonAsync("/api/v1/authorizedpeople", authorizedPersonDto);
            postAuthorizedPersonResponse.EnsureSuccessStatusCode();
            var createdAuthorizedPerson = await postAuthorizedPersonResponse.Content.ReadFromJsonAsync<AuthorizedPersonDto>();

            // 3. Lier les deux
            var linkDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = createdChild!.Id,
                AuthorizedPersonId = createdAuthorizedPerson!.Id
            };
            var linkResponse = await client.PostAsJsonAsync("/api/v1/linkauthorizedpersonchild", linkDto);
            linkResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/children/{createdChild.Id}/with-authorized-people");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAuthorizedPeopleDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdChild.Id);
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldContain(p => p.LastName == "Martin");
        }

        [Fact]
        public async Task GetChildWithAuthorizedPeople_ShouldReturnEmptyList_WhenNoAuthorizedPerson()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Sasha", Gender = "non_specifie" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/children/{createdChild!.Id}/with-authorized-people");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAuthorizedPeopleDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdChild.Id);
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildWithAuthorizedPeople_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/children/9999/with-authorized-people");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetChildWithAdditionalDatas_ShouldReturnData_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Créer un enfant
            var childDto = new CreateChildDto { LastName = "Jules", Gender = "m" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Créer une donnée supplémentaire liée à l’enfant
            var AdditionalDataDto = new CreateAdditionalDataDto
            {
                ChildId = createdChild!.Id,
                ParamName = "Allergie",
                ParamValue = "Gluten",
                ParamType = "Texte",
                Comment = "Important"
            };

            var postAdditionalDataResponse = await client.PostAsJsonAsync("/api/v1/additionaldatas", AdditionalDataDto);
            postAdditionalDataResponse.EnsureSuccessStatusCode();

            // Act
            var response = await client.GetAsync($"/api/v1/children/{createdChild.Id}/with-additional-datas");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAdditionalDatasDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdChild.Id);
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldContain(d => d.ParamName == "Allergie");
        }

        [Fact]
        public async Task GetChildWithAdditionalDatas_ShouldReturnEmptyList_WhenNoAdditionalData()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto { LastName = "Sami", Gender = "autre" };
            var postChildResponse = await client.PostAsJsonAsync("/api/v1/children", childDto);
            postChildResponse.EnsureSuccessStatusCode();
            var createdChild = await postChildResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.GetAsync($"/api/v1/children/{createdChild!.Id}/with-additional-datas");
            response.StatusCode.ShouldBe(HttpStatusCode.OK);

            var result = await response.Content.ReadFromJsonAsync<ChildWithAdditionalDatasDto>();

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(createdChild.Id);
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildWithAdditionalDatas_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.GetAsync("/api/v1/children/9999/with-additional-datas");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateChild_ShouldReturnCreated_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var childDto = new CreateChildDto
            {
                Gender = "mme",
                LastName = "Camille",
                FirstName = "Dupont"
            };

            // Act
            var response = await client.PostAsJsonAsync("/api/v1/children", childDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.Created);

            var created = await response.Content.ReadFromJsonAsync<ChildDto>();
            created.ShouldNotBeNull();
            created.LastName.ShouldBe("Camille");
            created.Gender.ShouldBe("mme");
        }

        [Fact]
        public async Task UpdateChild_ShouldReturnNoContent_WhenValid()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Emma", Gender = "mme" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/children", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            var updateDto = new UpdateChildDto
            {
                Id = created!.Id,
                LastName = "Emma Modifiée",
                Gender = "mme"
            };

            // Act
            var response = await client.PutAsJsonAsync($"/api/v1/children/{created.Id}", updateDto);

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/children/{created.Id}");
            var updated = await getResponse.Content.ReadFromJsonAsync<ChildDto>();
            updated.ShouldNotBeNull();
            updated!.LastName.ShouldBe("Emma Modifiée");
        }

        [Fact]
        public async Task UpdateChild_ShouldReturnBadRequest_WhenIdMismatch()
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
            var response = await client.PutAsJsonAsync("/api/v1/children/1", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.BadRequest);

            // Assert
            exception.Detail.ShouldBe("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");
        }

        [Fact]
        public async Task UpdateChild_ShouldReturnNotFound_WhenDoesNotExist()
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
            var response = await client.PutAsJsonAsync("/api/v1/children/9999", updateDto);
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task DeleteChild_ShouldReturnNoContent_WhenExists()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            var createDto = new CreateChildDto { LastName = "Lucas", Gender = "m" };
            var postResponse = await client.PostAsJsonAsync("/api/v1/children", createDto);
            postResponse.EnsureSuccessStatusCode();
            var created = await postResponse.Content.ReadFromJsonAsync<ChildDto>();

            // Act
            var response = await client.DeleteAsync($"/api/v1/children/{created!.Id}");

            // Assert
            response.EnsureSuccessStatusCode();
            response.StatusCode.ShouldBe(HttpStatusCode.NoContent);

            var getResponse = await client.GetAsync($"/api/v1/children/{created.Id}");
            getResponse.StatusCode.ShouldBe(HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task DeleteChild_ShouldReturnNotFound_WhenDoesNotExist()
        {
            // Arrange
            using var factory = new CustomWebApplicationFactory();
            var client = factory.CreateClient();

            // Act
            var response = await client.DeleteAsync($"/api/v1/children/9999");
            var exception = await AssertProblemDetails.AssertProblem(response, HttpStatusCode.NotFound);

            // Assert
            exception.Detail.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }
    }
}