using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class EnfantServiceTests
    {
        private readonly IChildService _enfantService;
        private readonly Mock<IChildRepository> _enfantRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public EnfantServiceTests()
        {
            _enfantRepositoryMock = new Mock<IChildRepository>();
            _mapperMock = new Mock<IMapper>();
            _enfantService = new ChildService(_enfantRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllEnfantsAsync_WhenEnfantsExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var enfants = new List<Enfant>
            {
                new() { Id = 1, Nom = "Alice" },
                new() { Id = 2, Nom = "Bob" }
            };

            var enfantsDto = new List<ChildDto>
            {
                new() { Id = 1, LastName = "Alice" },
                new() { Id = 2, LastName = "Bob" }
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(enfants);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ChildDto>>(enfants))
                .Returns(enfantsDto);

            // Act
            var result = await _enfantService.GetAllChildrenAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.LastName == "Alice");
        }

        [Fact]
        public async Task GetAllEnfantsAsync_WhenNoEnfants_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var enfants = new List<Enfant>();
            var enfantsDto = new List<ChildDto>();

            _enfantRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(enfants);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ChildDto>>(enfants))
                .Returns(enfantsDto);

            // Act
            var result = await _enfantService.GetAllChildrenAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantAsync_WhenEnfantExists_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant { Id = 1, Nom = "Alice" };
            var enfantDto = new ChildDto { Id = 1, LastName = "Alice" };

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildDto>(enfant))
                .Returns(enfantDto);

            // Act
            var result = await _enfantService.GetChildAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
        }

        [Fact]
        public async Task GetEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetChildAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetEnfantWithResponsablesAsync_WhenEnfantWithResponsablesExists_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant
            {
                Id = 1,
                Nom = "Alice",
                ResponsableEnfants = new List<ResponsableEnfant>
                {
                    new() { Responsable = new Responsable { Id = 1, Nom = "Bob" } },
                    new() { Responsable = new Responsable { Id = 2, Nom = "Charlie" } }
                }
            };

            var enfantWithResponsablesDto = new ChildWithGuardiansDto
            {
                Id = 1,
                LastName = "Alice",
                Guardians = new List<GuardianDto>
                {
                    new() { Id = 1, LastName = "Bob" },
                    new() { Id = 2, LastName = "Charlie" }
                }
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithGuardiansAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildWithGuardiansDto>(enfant))
                .Returns(enfantWithResponsablesDto);

            // Act
            var result = await _enfantService.GetChildWithGuardiansAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldNotBeEmpty();
            result.Guardians.Count.ShouldBe(2);
            result.Guardians.ShouldContain(r => r.LastName == "Bob");
        }

        [Fact]
        public async Task GetEnfantWithResponsablesAsync_WhenResponsablesListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant
            {
                Id = 1,
                Nom = "Alice",
                ResponsableEnfants = new List<ResponsableEnfant>()
            };

            var enfantWithResponsablesDto = new ChildWithGuardiansDto
            {
                Id = 1,
                LastName = "Alice",
                Guardians = new List<GuardianDto>()
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithGuardiansAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildWithGuardiansDto>(enfant))
                .Returns(enfantWithResponsablesDto);

            // Act
            var result = await _enfantService.GetChildWithGuardiansAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Alice");
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantWithResponsablesAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithGuardiansAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetChildWithGuardiansAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetEnfantWithPersonnesAutoriseesAsync_WhenEnfantWithPersonnesAutoriseesExists_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant
            {
                Id = 1,
                Nom = "Alice",
                PersonneAutoriseeEnfants = new List<PersonneAutoriseeEnfant>
                {
                    new() { PersonneAutorisee = new PersonneAutorisee { Id = 1, Nom = "Bob" } },
                    new() { PersonneAutorisee = new PersonneAutorisee { Id = 2, Nom = "Charlie" } }
                }
            };

            var enfantWithPersonnesAutoriseesDto = new ChildWithAuthorizedPeopleDto
            {
                Id = 1,
                LastName = "Alice",
                AuthorizedPeople = new List<AuthorizedPersonDto>
                {
                    new() { Id = 1, LastName = "Bob" },
                    new() { Id = 2, LastName = "Charlie" }
                }
            };
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithAuthorizedPeopleAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildWithAuthorizedPeopleDto>(enfant))
                .Returns(enfantWithPersonnesAutoriseesDto);

            // Act
            var result = await _enfantService.GetChildWithAuthorizedPeopleAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldNotBeEmpty();
            result.AuthorizedPeople.Count.ShouldBe(2);
            result.AuthorizedPeople.ShouldContain(r => r.LastName == "Bob");
        }

        [Fact]
        public async Task GetEnfantWithPersonnesAutoriseesAsync_WhenPersonnesAutoriseesListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant
            {
                Id = 1,
                Nom = "Alice",
                PersonneAutoriseeEnfants = new List<PersonneAutoriseeEnfant>()
            };

            var enfantWithPersonnesAutoriseesDto = new ChildWithAuthorizedPeopleDto
            {
                Id = 1,
                LastName = "Alice",
                AuthorizedPeople = new List<AuthorizedPersonDto>()
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithAuthorizedPeopleAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildWithAuthorizedPeopleDto>(enfant))
                .Returns(enfantWithPersonnesAutoriseesDto);

            // Act
            var result = await _enfantService.GetChildWithAuthorizedPeopleAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Alice");
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantWithPersonnesAutoriseesAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithAuthorizedPeopleAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetChildWithAuthorizedPeopleAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetEnfantWithDonneesSupplementairesAsync_WhenEnfantWithDonneesSupplementairesExists_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant
            {
                Id = 1,
                Nom = "Alice",
                DonneeSupplementaires = new List<DonneeSupplementaire>
                {
                    new() { Id = 1, Valeur = "A" },
                    new() { Id = 2, Valeur = "B" }
                }
            };

            var enfantWithDonneesSupplementairesDto = new ChildWithAdditionalDatasDto
            {
                Id = 1,
                LastName = "Alice",
                AdditionalDatas = new List<AdditionalDataDto>
                {
                    new() { Id = 1, ParamValue = "A" },
                    new() { Id = 2, ParamValue = "B" }
                }
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithAdditionalDatasAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildWithAdditionalDatasDto>(enfant))
                .Returns(enfantWithDonneesSupplementairesDto);

            // Act
            var result = await _enfantService.GetChildWithAdditionalDatasAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldNotBeEmpty();
            result.AdditionalDatas.Count.ShouldBe(2);
            result.AdditionalDatas.ShouldContain(r => r.ParamValue == "A");
        }

        [Fact]
        public async Task GetEnfantWithDonneesSupplementairesAsync_WhenDonneesSupplementairesListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant
            {
                Id = 1,
                Nom = "Alice",
                DonneeSupplementaires = new List<DonneeSupplementaire>()
            };

            var enfantWithDonneesSupplementairesDto = new ChildWithAdditionalDatasDto
            {
                Id = 1,
                LastName = "Alice",
                AdditionalDatas = new List<AdditionalDataDto>()
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithAdditionalDatasAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildWithAdditionalDatasDto>(enfant))
                .Returns(enfantWithDonneesSupplementairesDto);

            // Act
            var result = await _enfantService.GetChildWithAdditionalDatasAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Alice");
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantWithDonneesSupplementairesAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithAdditionalDatasAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetChildWithAdditionalDatasAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateEnfantAsync_WhenEnfantIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newEnfantDto = new CreateChildDto { LastName = "Alice" };
            var enfant = new Enfant { Id = 1, Nom = "Alice" };
            var createdEnfantDto = new ChildDto { Id = 1, LastName = "Alice" };

            _mapperMock
                .Setup(m => m.Map<Enfant>(newEnfantDto))
                .Returns(enfant);

            _enfantRepositoryMock
                .Setup(repo => repo.AddAsync(enfant))
                .Returns(Task.CompletedTask);

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(enfant.Id))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<ChildDto>(enfant))
                .Returns(createdEnfantDto);

            // Act
            var result = await _enfantService.CreateChildAsync(newEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");

            _enfantRepositoryMock.Verify(repo => repo.AddAsync(enfant), Times.Once);
        }

        [Fact]
        public async Task CreateEnfantAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newEnfantDto = new CreateChildDto { LastName = "Alice" };

            _mapperMock
                .Setup(m => m.Map<Enfant>(newEnfantDto))
                .Returns((Enfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.CreateChildAsync(newEnfantDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de l'enfant : Le Mapping a échoué.");

            _enfantRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Enfant>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEnfantAsync_WhenEnfantExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateEnfantDto = new UpdateChildDto { Id = 1, LastName = "Alice" };
            var enfant = new Enfant { Id = id, Nom = "Alice" };

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map(updateEnfantDto, enfant))
                .Returns(enfant);

            _enfantRepositoryMock
                .Setup(repo => repo.UpdateAsync(enfant))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _enfantService.UpdateChildAsync(id, updateEnfantDto));

            _enfantRepositoryMock.Verify(r => r.UpdateAsync(enfant), Times.Once);
        }

        [Fact]
        public async Task UpdateEnfantAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateEnfantDto = new UpdateChildDto { Id = 2, LastName = "Alice" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.UpdateChildAsync(id, updateEnfantDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");

            _enfantRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Enfant>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateEnfantDto = new UpdateChildDto { Id = id, LastName = "Alice" };

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.UpdateChildAsync(id, updateEnfantDto));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");

            _enfantRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Enfant>()), Times.Never);
        }

        [Fact]
        public async Task DeleteEnfantAsync_WhenEnfantExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var enfant = new Enfant { Id = id, Nom = "Alice" };

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(enfant);

            _enfantRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _enfantService.DeleteChildAsync(id));

            _enfantRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.DeleteChildAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");

            _enfantRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}