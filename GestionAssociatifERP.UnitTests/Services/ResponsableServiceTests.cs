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
        private readonly IGuardianService _responsableService;
        private readonly Mock<IGuardianRepository> _responsableRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public ResponsableServiceTests()
        {
            _responsableRepositoryMock = new Mock<IGuardianRepository>();
            _mapperMock = new Mock<IMapper>();
            _responsableService = new GuardianService(_responsableRepositoryMock.Object, _mapperMock.Object);
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

            var responsablesDto = new List<GuardianDto>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(responsables);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<GuardianDto>>(responsables))
                .Returns(responsablesDto);

            // Act
            var result = await _responsableService.GetAllGuardiansAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.LastName == "Doe");
        }

        [Fact]
        public async Task GetAllResponsables_WhenNoResponsables_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var responsables = new List<Responsable>();
            var responsablesDto = new List<GuardianDto>();

            _responsableRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(responsables);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<GuardianDto>>(responsables))
                .Returns(responsablesDto);

            // Act
            var result = await _responsableService.GetAllGuardiansAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetResponsableAsync_WhenResponsableExists_ShouldReturnMappedDto()
        {
            // Arrange
            var responsable = new Responsable { Id = 1, Prenom = "John", Nom = "Doe" };
            var responsableDto = new GuardianDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<GuardianDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetGuardianAsync(1);

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
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetGuardianAsync(1));

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

            var responsableDto = new GuardianWithFinancialInformationDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                FinancialInformation = new FinancialInformationDto
                {
                    Id = 1,
                    FamilyQuotient = 1000
                }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithFinancialInformationAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<GuardianWithFinancialInformationDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetGuardianWithFinancialInformationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Doe");
            result.FinancialInformation.ShouldNotBeNull();
            result.FinancialInformation.Id.ShouldBe(1);
            result.FinancialInformation.FamilyQuotient.ShouldBe(1000);
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

            var responsableDto = new GuardianWithFinancialInformationDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                FinancialInformation = null
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithFinancialInformationAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<GuardianWithFinancialInformationDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetGuardianWithFinancialInformationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Doe");
            result.FinancialInformation.ShouldBeNull();
        }

        [Fact]
        public async Task GetResponsableWithInformationFinanciereAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(repo => repo.GetWithFinancialInformationAsync(1))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetGuardianWithFinancialInformationAsync(1));

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

            var responsableDto = new GuardianWithPersonalSituationDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalSituation = new PersonalSituationDto
                {
                    Id = 1,
                    FamilySituation = "Célibataire"
                }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithPersonalSituationAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<GuardianWithPersonalSituationDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetGuardianWithPersonalSituationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.PersonalSituation.ShouldNotBeNull();
            result.PersonalSituation.Id.ShouldBe(1);
            result.PersonalSituation.FamilySituation.ShouldBe("Célibataire");
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

            var responsableDto = new GuardianWithPersonalSituationDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalSituation = null
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithPersonalSituationAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<GuardianWithPersonalSituationDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetGuardianWithPersonalSituationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.PersonalSituation.ShouldBeNull();
        }

        [Fact]
        public async Task GetResponsableWithSituationPersonnelleAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(repo => repo.GetWithPersonalSituationAsync(1))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetGuardianWithPersonalSituationAsync(1));

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

            var responsableDto = new GuardianWithChildrenDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Children = new List<ChildDto>
                {
                    new() { Id = 1, FirstName = "Jane", LastName = "Doe" },
                    new() { Id = 2, FirstName = "Jack", LastName = "Doe" }
                }
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<GuardianWithChildrenDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetGuardianWithChildrenAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.Children.ShouldNotBeNull();
            result.Children.Count.ShouldBe(2);
            result.Children.ShouldContain(e => e.FirstName == "Jane" && e.LastName == "Doe");
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

            var responsableDto = new GuardianWithChildrenDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Children = new List<ChildDto>()
            };

            _responsableRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(responsable);

            _mapperMock
                .Setup(m => m.Map<GuardianWithChildrenDto>(responsable))
                .Returns(responsableDto);

            // Act
            var result = await _responsableService.GetGuardianWithChildrenAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.Children.ShouldNotBeNull();
            result.Children.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetResponsableWithEnfantsAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.GetGuardianWithChildrenAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateResponsableAsync_WhenResponsableIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newResponsableDto = new CreateGuardianDto { FirstName = "John", LastName = "Doe" };
            var responsable = new Responsable { Id = 1, Prenom = "John", Nom = "Doe" };
            var createdResponsableDto = new GuardianDto { Id = 1, FirstName = "John", LastName = "Doe" };

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
                .Setup(m => m.Map<GuardianDto>(responsable))
                .Returns(createdResponsableDto);

            // Act
            var result = await _responsableService.CreateGuardianAsync(newResponsableDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");

            _responsableRepositoryMock.Verify(repo => repo.AddAsync(responsable), Times.Once);
        }

        [Fact]
        public async Task CreateResponsableAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newResponsableDto = new CreateGuardianDto { FirstName = "John", LastName = "Doe" };

            _mapperMock
                .Setup(m => m.Map<Responsable>(newResponsableDto))
                .Returns((Responsable)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.CreateGuardianAsync(newResponsableDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du responsable : Le Mapping a échoué.");

            _responsableRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Responsable>()), Times.Never);
        }

        [Fact]
        public async Task UpdateResponsableAsync_WhenEnfantExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateResponsableDto = new UpdateGuardianDto { Id = 1, FirstName = "John", LastName = "Doe" };
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
            await Should.NotThrowAsync(async () => await _responsableService.UpdateGuardianAsync(id, updateResponsableDto));

            _responsableRepositoryMock.Verify(repo => repo.UpdateAsync(responsable), Times.Once);
        }

        [Fact]
        public async Task UpdateResponsableAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateResponsableDto = new UpdateGuardianDto { Id = 2, FirstName = "John", LastName = "Doe" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.UpdateGuardianAsync(id, updateResponsableDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant du responsable ne correspond pas à celui de l'objet envoyé.");

            _responsableRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Responsable>()), Times.Never);
        }

        [Fact]
        public async Task UpdateResponsableAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateResponsableDto = new UpdateGuardianDto { Id = id, FirstName = "John", LastName = "Doe" };

            _responsableRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Responsable);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.UpdateGuardianAsync(id, updateResponsableDto));

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
            await Should.NotThrowAsync(async () => await _responsableService.DeleteGuardianAsync(id));

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
            var exception = await Should.ThrowAsync<Exception>(async () => await _responsableService.DeleteGuardianAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");

            _responsableRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}