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
        private readonly IEnfantService _enfantService;
        private readonly Mock<IEnfantRepository> _enfantRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public EnfantServiceTests()
        {
            _enfantRepositoryMock = new Mock<IEnfantRepository>();
            _mapperMock = new Mock<IMapper>();
            _enfantService = new EnfantService(_enfantRepositoryMock.Object, _mapperMock.Object);
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

            var enfantsDto = new List<EnfantDto>
            {
                new() { Id = 1, Nom = "Alice" },
                new() { Id = 2, Nom = "Bob" }
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(enfants);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<EnfantDto>>(enfants))
                .Returns(enfantsDto);

            // Act
            var result = await _enfantService.GetAllEnfantsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.Nom == "Alice");
        }

        [Fact]
        public async Task GetAllEnfantsAsync_WhenNoEnfants_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var enfants = new List<Enfant>();
            var enfantsDto = new List<EnfantDto>();

            _enfantRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(enfants);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<EnfantDto>>(enfants))
                .Returns(enfantsDto);

            // Act
            var result = await _enfantService.GetAllEnfantsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantAsync_WhenEnfantExists_ShouldReturnMappedDto()
        {
            // Arrange
            var enfant = new Enfant { Id = 1, Nom = "Alice" };
            var enfantDto = new EnfantDto { Id = 1, Nom = "Alice" };

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<EnfantDto>(enfant))
                .Returns(enfantDto);

            // Act
            var result = await _enfantService.GetEnfantAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Nom.ShouldBe("Alice");
        }

        [Fact]
        public async Task GetEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetEnfantAsync(1));

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

            var enfantWithResponsablesDto = new EnfantWithResponsablesDto
            {
                Id = 1,
                Nom = "Alice",
                Responsables = new List<ResponsableDto>
                {
                    new() { Id = 1, Nom = "Bob" },
                    new() { Id = 2, Nom = "Charlie" }
                }
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithResponsablesAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<EnfantWithResponsablesDto>(enfant))
                .Returns(enfantWithResponsablesDto);

            // Act
            var result = await _enfantService.GetEnfantWithResponsablesAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Nom.ShouldBe("Alice");
            result.Responsables.ShouldNotBeNull();
            result.Responsables.ShouldNotBeEmpty();
            result.Responsables.Count.ShouldBe(2);
            result.Responsables.ShouldContain(r => r.Nom == "Bob");
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

            var enfantWithResponsablesDto = new EnfantWithResponsablesDto
            {
                Id = 1,
                Nom = "Alice",
                Responsables = new List<ResponsableDto>()
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithResponsablesAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<EnfantWithResponsablesDto>(enfant))
                .Returns(enfantWithResponsablesDto);

            // Act
            var result = await _enfantService.GetEnfantWithResponsablesAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Nom.ShouldBe("Alice");
            result.Responsables.ShouldNotBeNull();
            result.Responsables.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantWithResponsablesAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithResponsablesAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetEnfantWithResponsablesAsync(1));

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

            var enfantWithPersonnesAutoriseesDto = new EnfantWithPersonnesAutoriseesDto
            {
                Id = 1,
                Nom = "Alice",
                PersonnesAutorisees = new List<PersonneAutoriseeDto>
                {
                    new() { Id = 1, Nom = "Bob" },
                    new() { Id = 2, Nom = "Charlie" }
                }
            };
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithPersonnesAutoriseesAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<EnfantWithPersonnesAutoriseesDto>(enfant))
                .Returns(enfantWithPersonnesAutoriseesDto);

            // Act
            var result = await _enfantService.GetEnfantWithPersonnesAutoriseesAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Nom.ShouldBe("Alice");
            result.PersonnesAutorisees.ShouldNotBeNull();
            result.PersonnesAutorisees.ShouldNotBeEmpty();
            result.PersonnesAutorisees.Count.ShouldBe(2);
            result.PersonnesAutorisees.ShouldContain(r => r.Nom == "Bob");
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

            var enfantWithPersonnesAutoriseesDto = new EnfantWithPersonnesAutoriseesDto
            {
                Id = 1,
                Nom = "Alice",
                PersonnesAutorisees = new List<PersonneAutoriseeDto>()
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithPersonnesAutoriseesAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<EnfantWithPersonnesAutoriseesDto>(enfant))
                .Returns(enfantWithPersonnesAutoriseesDto);

            // Act
            var result = await _enfantService.GetEnfantWithPersonnesAutoriseesAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Nom.ShouldBe("Alice");
            result.PersonnesAutorisees.ShouldNotBeNull();
            result.PersonnesAutorisees.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantWithPersonnesAutoriseesAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithPersonnesAutoriseesAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetEnfantWithPersonnesAutoriseesAsync(1));

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

            var enfantWithDonneesSupplementairesDto = new EnfantWithDonneesSupplementairesDto
            {
                Id = 1,
                Nom = "Alice",
                DonneeSupplementaires = new List<DonneeSupplementaireDto>
                {
                    new() { Id = 1, Valeur = "A" },
                    new() { Id = 2, Valeur = "B" }
                }
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithDonneesSupplementairesAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<EnfantWithDonneesSupplementairesDto>(enfant))
                .Returns(enfantWithDonneesSupplementairesDto);

            // Act
            var result = await _enfantService.GetEnfantWithDonneesSupplementairesAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Nom.ShouldBe("Alice");
            result.DonneeSupplementaires.ShouldNotBeNull();
            result.DonneeSupplementaires.ShouldNotBeEmpty();
            result.DonneeSupplementaires.Count.ShouldBe(2);
            result.DonneeSupplementaires.ShouldContain(r => r.Valeur == "A");
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

            var enfantWithDonneesSupplementairesDto = new EnfantWithDonneesSupplementairesDto
            {
                Id = 1,
                Nom = "Alice",
                DonneeSupplementaires = new List<DonneeSupplementaireDto>()
            };

            _enfantRepositoryMock
                .Setup(repo => repo.GetWithDonneesSupplementairesAsync(1))
                .ReturnsAsync(enfant);

            _mapperMock
                .Setup(m => m.Map<EnfantWithDonneesSupplementairesDto>(enfant))
                .Returns(enfantWithDonneesSupplementairesDto);

            // Act
            var result = await _enfantService.GetEnfantWithDonneesSupplementairesAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Nom.ShouldBe("Alice");
            result.DonneeSupplementaires.ShouldNotBeNull();
            result.DonneeSupplementaires.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantWithDonneesSupplementairesAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(repo => repo.GetWithDonneesSupplementairesAsync(1))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.GetEnfantWithDonneesSupplementairesAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateEnfantAsync_WhenEnfantIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newEnfantDto = new CreateEnfantDto { Nom = "Alice" };
            var enfant = new Enfant { Id = 1, Nom = "Alice" };
            var createdEnfantDto = new EnfantDto { Id = 1, Nom = "Alice" };

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
                .Setup(m => m.Map<EnfantDto>(enfant))
                .Returns(createdEnfantDto);

            // Act
            var result = await _enfantService.CreateEnfantAsync(newEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Nom.ShouldBe("Alice");

            _enfantRepositoryMock.Verify(repo => repo.AddAsync(enfant), Times.Once);
        }

        [Fact]
        public async Task CreateEnfantAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newEnfantDto = new CreateEnfantDto { Nom = "Alice" };

            _mapperMock
                .Setup(m => m.Map<Enfant>(newEnfantDto))
                .Returns((Enfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.CreateEnfantAsync(newEnfantDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de l'enfant : Le Mapping a échoué.");

            _enfantRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Enfant>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEnfantAsync_WhenEnfantExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateEnfantDto = new UpdateEnfantDto { Id = 1, Nom = "Alice" };
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
            await Should.NotThrowAsync(async () => await _enfantService.UpdateEnfantAsync(id, updateEnfantDto));

            _enfantRepositoryMock.Verify(r => r.UpdateAsync(enfant), Times.Once);
        }

        [Fact]
        public async Task UpdateEnfantAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateEnfantDto = new UpdateEnfantDto { Id = 2, Nom = "Alice" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.UpdateEnfantAsync(id, updateEnfantDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");

            _enfantRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Enfant>()), Times.Never);
        }

        [Fact]
        public async Task UpdateEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateEnfantDto = new UpdateEnfantDto { Id = id, Nom = "Alice" };

            _enfantRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Enfant);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.UpdateEnfantAsync(id, updateEnfantDto));

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
            await Should.NotThrowAsync(async () => await _enfantService.DeleteEnfantAsync(id));

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
            var exception = await Should.ThrowAsync<Exception>(async () => await _enfantService.DeleteEnfantAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");

            _enfantRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}