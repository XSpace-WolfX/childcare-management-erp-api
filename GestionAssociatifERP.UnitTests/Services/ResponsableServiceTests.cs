using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class ResponsableServiceTests
    {
        private readonly IResponsableService _responsableService;
        private readonly Mock<IResponsableRepository> _responsableRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public ResponsableServiceTests()
        {
            _responsableRepositoryMock = new Mock<IResponsableRepository>();
            _mapperMock = new Mock<IMapper>();
            _responsableService = new ResponsableService(_responsableRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllResponsables_WhenResponsablesExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var responsables = new List<Responsable>
            {
                new() { Id = 1, Prenom = "John", Nom = "Doe" },
                new() { Id = 2, Prenom = "Jane", Nom = "Smith" }
            };

            var responsablesDto = new List<ResponsableDto>
            {
                new() { Id = 1, Prenom = "John", Nom = "Doe" },
                new() { Id = 2, Prenom = "Jane", Nom = "Smith" }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(responsables);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ResponsableDto>>(responsables))
                .Returns(responsablesDto);

            // Act
            var result = await _responsableService.GetAllResponsablesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.Nom == "Doe");
        }

        [Fact]
        public async Task GetAllResponsables_WhenNoResponsables_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var responsables = new List<Responsable>();
            var responsablesDto = new List<ResponsableDto>();

            _responsableRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(responsables);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ResponsableDto>>(responsables))
                .Returns(responsablesDto);

            // Act
            var result = await _responsableService.GetAllResponsablesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetResponsableAsync_WhenResponsableExists_ShouldReturnMappedDto()
        {
            // Arrange
            var responsable = new Responsable { Id = 1, Prenom = "John", Nom = "Doe" };
            var responsableDto = new ResponsableDto { Id = 1, Prenom = "John", Nom = "Doe" };

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetResponsableAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public async Task GetResponsableAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetResponsableAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetResponsableWithInformationFinanciereAsync_WhenResponsableWithInformationFinanciereExists_ShouldReturnMappedDto()
        {
            // Arrange
            var responsable = new Responsable
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                InformationFinanciere = new InformationFinanciere
                {
                    Id = 1,
                    QuotientFamiliale = 1000
                }
            };

            var responsableDto = new ResponsableWithInformationFinanciereDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                InformationFinanciere = new InformationFinanciereDto
                {
                    Id = 1,
                    QuotientFamiliale = 1000
                }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithInformationFinanciereAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableWithInformationFinanciereDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetResponsableWithInformationFinanciereAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Nom.ShouldBe("Doe");
            result.InformationFinanciere.ShouldNotBeNull();
            result.InformationFinanciere.Id.ShouldBe(1);
            result.InformationFinanciere.QuotientFamiliale.ShouldBe(1000);
        }

        [Fact]
        public async Task GetResponsableWithInformationFinanciereAsync_WhenInformationFinanciereDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var responsable = new Responsable
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe"
            };

            var responsableDto = new ResponsableWithInformationFinanciereDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                InformationFinanciere = null
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithInformationFinanciereAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableWithInformationFinanciereDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetResponsableWithInformationFinanciereAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Nom.ShouldBe("Doe");
            result.InformationFinanciere.ShouldBeNull();
        }

        [Fact]
        public async Task GetResponsableWithInformationFinanciereAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(repo => repo.GetWithInformationFinanciereAsync(1))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetResponsableWithInformationFinanciereAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetResponsableWithSituationPersonnelleAsync_WhenResponsableWithSituationPersonnelleExists_ShouldReturnMappedDto()
        {
            // Arrange
            var responsable = new Responsable
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                SituationPersonnelle = new SituationPersonnelle
                {
                    Id = 1,
                    SituationFamiliale = "Célibataire"
                }
            };

            var responsableDto = new ResponsableWithSituationPersonnelleDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                SituationPersonnelle = new SituationPersonnelleDto
                {
                    Id = 1,
                    SituationFamiliale = "Célibataire"
                }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithSituationPersonnelleAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableWithSituationPersonnelleDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetResponsableWithSituationPersonnelleAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");
            result.SituationPersonnelle.ShouldNotBeNull();
            result.SituationPersonnelle.Id.ShouldBe(1);
            result.SituationPersonnelle.SituationFamiliale.ShouldBe("Célibataire");
        }

        [Fact]
        public async Task GetResponsableWithSituationPersonnelleAsync_WhenSituationPersonnelleDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var responsable = new Responsable
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe"
            };

            var responsableDto = new ResponsableWithSituationPersonnelleDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                SituationPersonnelle = null
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithSituationPersonnelleAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableWithSituationPersonnelleDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetResponsableWithSituationPersonnelleAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");
            result.SituationPersonnelle.ShouldBeNull();
        }

        [Fact]
        public async Task GetResponsableWithSituationPersonnelleAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(repo => repo.GetWithSituationPersonnelleAsync(1))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetResponsableWithSituationPersonnelleAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetResponsableWithEnfantsAsync_WhenResponsableWithEnfantsExists_ShouldReturnMappedDto()
        {
            // Arrange
            var responsable = new Responsable
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                ResponsableEnfants = new List<ResponsableEnfant>
                {
                    new() { Enfant = new Enfant { Id = 1, Prenom = "Jane", Nom = "Doe" } },
                    new() { Enfant = new Enfant { Id = 2, Prenom = "Jack", Nom = "Doe" } }
                }
            };

            var responsableDto = new ResponsableWithEnfantsDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                Enfants = new List<EnfantDto>
                {
                    new() { Id = 1, Prenom = "Jane", Nom = "Doe" },
                    new() { Id = 2, Prenom = "Jack", Nom = "Doe" }
                }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithEnfantsAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableWithEnfantsDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetResponsableWithEnfantsAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");
            result.Enfants.ShouldNotBeNull();
            result.Enfants.Count.ShouldBe(2);
            result.Enfants.ShouldContain(e => e.Prenom == "Jane" && e.Nom == "Doe");
        }

        [Fact]
        public async Task GetResponsableWithEnfantsAsync_WhenEnfantsListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var responsable = new Responsable
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                ResponsableEnfants = new List<ResponsableEnfant>()
            };

            var responsableDto = new ResponsableWithEnfantsDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                Enfants = new List<EnfantDto>()
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithEnfantsAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableWithEnfantsDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetResponsableWithEnfantsAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");
            result.Enfants.ShouldNotBeNull();
            result.Enfants.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetResponsableWithEnfantsAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(repo => repo.GetWithEnfantsAsync(1))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetResponsableWithEnfantsAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateResponsableAsync_WhenResponsableIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newResponsableDto = new CreateResponsableDto { Prenom = "John", Nom = "Doe" };
            var responsable = new Responsable { Id = 1, Prenom = "John", Nom = "Doe" };
            var createdResponsableDto = new ResponsableDto { Id = 1, Prenom = "John", Nom = "Doe" };

            _mapperMock
                .Setup(m => m.Map<Responsable>(newResponsableDto))
                .Returns(responsable);

            _responsableRepositoryMock
                .Setup(repo => repo.AddAsync(responsable))
                .Returns(Task.CompletedTask);

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(responsable.Id))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<ResponsableDto>(responsable))
                .Returns(createdResponsableDto);

            // Act
            var result = await _responsableService.CreateResponsableAsync(newResponsableDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");

            _responsableRepositoryMock.Verify(repo => repo.AddAsync(responsable), Times.Once);
        }

        [Fact]
        public async Task CreateResponsableAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newResponsableDto = new CreateResponsableDto { Prenom = "John", Nom = "Doe" };

            _mapperMock
                .Setup(m => m.Map<Responsable>(newResponsableDto))
                .Returns((Responsable)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.CreateResponsableAsync(newResponsableDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du responsable : Le Mapping a échoué.");

            _responsableRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Responsable>()), Times.Never);
        }

        [Fact]
        public async Task UpdateResponsableAsync_WhenEnfantExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateResponsableDto = new UpdateResponsableDto { Id = 1, Prenom = "John", Nom = "Doe" };
            var responsable = new Responsable { Id = id, Prenom = "John", Nom = "Doe" };

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map(updateResponsableDto, responsable))
                .Returns(responsable);

            _responsableRepositoryMock
                .Setup(repo => repo.UpdateAsync(responsable))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _responsableService.UpdateResponsableAsync(id, updateResponsableDto));

            _responsableRepositoryMock.Verify(repo => repo.UpdateAsync(responsable), Times.Once);
        }

        [Fact]
        public async Task UpdateResponsableAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateResponsableDto = new UpdateResponsableDto { Id = 2, Prenom = "John", Nom = "Doe" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.UpdateResponsableAsync(id, updateResponsableDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant du responsable ne correspond pas à celui de l'objet envoyé.");

            _responsableRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Responsable>()), Times.Never);
        }

        [Fact]
        public async Task UpdateResponsableAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateResponsableDto = new UpdateResponsableDto { Id = id, Prenom = "John", Nom = "Doe" };

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.UpdateResponsableAsync(id, updateResponsableDto));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");

            _responsableRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Responsable>()), Times.Never);
        }

        [Fact]
        public async Task DeleteResponsableAsync_WhenResponsableExists_ShouldReturnTrue()
        {
            // Arrange
            var id = 1;
            var responsable = new Responsable { Id = id, Prenom = "John", Nom = "Doe" };

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(responsable);

            _responsableRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _responsableService.DeleteResponsableAsync(id));

            _responsableRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteResponsableAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.DeleteResponsableAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");

            _responsableRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}