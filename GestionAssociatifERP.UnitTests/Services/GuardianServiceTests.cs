using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Infrastructure.Persistence.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class GuardianServiceTests
    {
        private readonly IGuardianService _guardianService;
        private readonly Mock<IGuardianRepository> _guardianRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public GuardianServiceTests()
        {
            _guardianRepositoryMock = new Mock<IGuardianRepository>();
            _mapperMock = new Mock<IMapper>();
            _guardianService = new GuardianService(_guardianRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllGuardians_WhenGuardiansExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var guardians = new List<Guardian>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            var guardiansDto = new List<GuardianDto>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Smith" }
            };

            _guardianRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(guardians);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<GuardianDto>>(guardians))
                .Returns(guardiansDto);

            // Act
            var result = await _guardianService.GetAllGuardiansAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.LastName == "Doe");
        }

        [Fact]
        public async Task GetAllGuardians_WhenNoGuardian_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var guardians = new List<Guardian>();
            var guardiansDto = new List<GuardianDto>();

            _guardianRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(guardians);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<GuardianDto>>(guardians))
                .Returns(guardiansDto);

            // Act
            var result = await _guardianService.GetAllGuardiansAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetGuardianAsync_WhenGuardianExists_ShouldReturnMappedDto()
        {
            // Arrange
            var guardian = new Guardian { Id = 1, FirstName = "John", LastName = "Doe" };
            var guardianDto = new GuardianDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _guardianRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianDto>(guardian))
                .Returns(guardianDto);

            // Act
            var result = await _guardianService.GetGuardianAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
        }

        [Fact]
        public async Task GetGuardianAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _guardianRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as Guardian);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.GetGuardianAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetGuardianWithFinancialInformationAsync_WhenGuardianWithFinancialInformationExists_ShouldReturnMappedDto()
        {
            // Arrange
            var guardian = new Guardian
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                FinancialInformations = new List<FinancialInformation>
                {
                    new() {
                    Id = 1,
                    FamilyQuotient = 1000
                    }
                }
            };

            var guardianDto = new GuardianWithFinancialInformationDto
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

            _guardianRepositoryMock
                .Setup(repo => repo.GetWithFinancialInformationAsync(1))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianWithFinancialInformationDto>(guardian))
                .Returns(guardianDto);

            // Act
            var result = await _guardianService.GetGuardianWithFinancialInformationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Doe");
            result.FinancialInformation.ShouldNotBeNull();
            result.FinancialInformation.Id.ShouldBe(1);
            result.FinancialInformation.FamilyQuotient.ShouldBe(1000);
        }

        [Fact]
        public async Task GetGuardianWithFinancialInformationAsync_WhenFinancialInformationDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var guardian = new Guardian
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            var guardianDto = new GuardianWithFinancialInformationDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                FinancialInformation = null
            };

            _guardianRepositoryMock
                .Setup(repo => repo.GetWithFinancialInformationAsync(1))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianWithFinancialInformationDto>(guardian))
                .Returns(guardianDto);

            // Act
            var result = await _guardianService.GetGuardianWithFinancialInformationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Doe");
            result.FinancialInformation.ShouldBeNull();
        }

        [Fact]
        public async Task GetGuardianWithFinancialInformationAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _guardianRepositoryMock
                .Setup(repo => repo.GetWithFinancialInformationAsync(1))
                .ReturnsAsync(null as Guardian);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.GetGuardianWithFinancialInformationAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetGuardianWithPersonalSituationAsync_WhenGuardianWithPersonalSituationExists_ShouldReturnMappedDto()
        {
            // Arrange
            var guardian = new Guardian
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalSituations = new List<PersonalSituation>
                {
                    new() {
                    Id = 1,
                    MaritalStatus = "Célibataire"
                    }
                }
            };

            var guardianDto = new GuardianWithPersonalSituationDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalSituation = new PersonalSituationDto
                {
                    Id = 1,
                    MaritalStatus = "Célibataire"
                }
            };

            _guardianRepositoryMock
                .Setup(repo => repo.GetWithPersonalSituationAsync(1))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianWithPersonalSituationDto>(guardian))
                .Returns(guardianDto);

            // Act
            var result = await _guardianService.GetGuardianWithPersonalSituationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.PersonalSituation.ShouldNotBeNull();
            result.PersonalSituation.Id.ShouldBe(1);
            result.PersonalSituation.MaritalStatus.ShouldBe("Célibataire");
        }

        [Fact]
        public async Task GetGuardianWithPersonalSituationAsync_WhenPersonalSituationDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var guardian = new Guardian
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe"
            };

            var guardianDto = new GuardianWithPersonalSituationDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                PersonalSituation = null
            };

            _guardianRepositoryMock
                .Setup(repo => repo.GetWithPersonalSituationAsync(1))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianWithPersonalSituationDto>(guardian))
                .Returns(guardianDto);

            // Act
            var result = await _guardianService.GetGuardianWithPersonalSituationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.PersonalSituation.ShouldBeNull();
        }

        [Fact]
        public async Task GetGuardianWithPersonalSituationAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _guardianRepositoryMock
                .Setup(repo => repo.GetWithPersonalSituationAsync(1))
                .ReturnsAsync(null as Guardian);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.GetGuardianWithPersonalSituationAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetGuardianWithChildrenAsync_WhenGuardianWithChildrenExists_ShouldReturnMappedDto()
        {
            // Arrange
            var guardian = new Guardian
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                GuardianChildren = new List<GuardianChild>
                {
                    new() { Child = new Child { Id = 1, FirstName = "Jane", LastName = "Doe" } },
                    new() { Child = new Child { Id = 2, FirstName = "Jack", LastName = "Doe" } }
                }
            };

            var guardianDto = new GuardianWithChildrenDto
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

            _guardianRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianWithChildrenDto>(guardian))
                .Returns(guardianDto);

            // Act
            var result = await _guardianService.GetGuardianWithChildrenAsync(1);

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
        public async Task GetGuardianWithChildrenAsync_WhenChildListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var guardian = new Guardian
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                GuardianChildren = new List<GuardianChild>()
            };

            var guardianWithChildrenDto = new GuardianWithChildrenDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Children = new List<ChildDto>()
            };

            _guardianRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianWithChildrenDto>(guardian))
                .Returns(guardianWithChildrenDto);

            // Act
            var result = await _guardianService.GetGuardianWithChildrenAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.Children.ShouldNotBeNull();
            result.Children.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetGuardianWithChildrenAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _guardianRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(null as Guardian);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.GetGuardianWithChildrenAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateGuardianAsync_WhenGuardianIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newGuardianDto = new CreateGuardianDto { FirstName = "John", LastName = "Doe" };
            var guardian = new Guardian { Id = 1, FirstName = "John", LastName = "Doe" };
            var createdGuardianDto = new GuardianDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _mapperMock
                .Setup(m => m.Map<Guardian>(newGuardianDto))
                .Returns(guardian);

            _guardianRepositoryMock
                .Setup(repo => repo.AddAsync(guardian))
                .Returns(Task.CompletedTask);

            _guardianRepositoryMock
                .Setup(repo => repo.GetByIdAsync(guardian.Id))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map<GuardianDto>(guardian))
                .Returns(createdGuardianDto);

            // Act
            var result = await _guardianService.CreateGuardianAsync(newGuardianDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");

            _guardianRepositoryMock.Verify(repo => repo.AddAsync(guardian), Times.Once);
        }

        [Fact]
        public async Task CreateGuardianAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newGuardianDto = new CreateGuardianDto { FirstName = "John", LastName = "Doe" };

            _mapperMock
                .Setup(m => m.Map<Guardian>(newGuardianDto))
                .Returns((Guardian)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.CreateGuardianAsync(newGuardianDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du responsable : Le Mapping a échoué.");

            _guardianRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Guardian>()), Times.Never);
        }

        [Fact]
        public async Task UpdateGuardianAsync_WhenGuardianExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateGuardianDto = new UpdateGuardianDto { Id = 1, FirstName = "John", LastName = "Doe" };
            var guardian = new Guardian { Id = id, FirstName = "John", LastName = "Doe" };

            _guardianRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(guardian);

            _mapperMock
                .Setup(m => m.Map(updateGuardianDto, guardian))
                .Returns(guardian);

            _guardianRepositoryMock
                .Setup(repo => repo.UpdateAsync(guardian))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _guardianService.UpdateGuardianAsync(id, updateGuardianDto));

            _guardianRepositoryMock.Verify(repo => repo.UpdateAsync(guardian), Times.Once);
        }

        [Fact]
        public async Task UpdateGuardianAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateGuardianDto = new UpdateGuardianDto { Id = 2, FirstName = "John", LastName = "Doe" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.UpdateGuardianAsync(id, updateGuardianDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant du responsable ne correspond pas à celui de l'objet envoyé.");

            _guardianRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Guardian>()), Times.Never);
        }

        [Fact]
        public async Task UpdateGuardianAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateGuardianDto = new UpdateGuardianDto { Id = id, FirstName = "John", LastName = "Doe" };

            _guardianRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Guardian);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.UpdateGuardianAsync(id, updateGuardianDto));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");

            _guardianRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<Guardian>()), Times.Never);
        }

        [Fact]
        public async Task DeleteGuardianAsync_WhenGuardianExists_ShouldReturnTrue()
        {
            // Arrange
            var id = 1;
            var guardian = new Guardian { Id = id, FirstName = "John", LastName = "Doe" };

            _guardianRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(guardian);

            _guardianRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _guardianService.DeleteGuardianAsync(id));

            _guardianRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteGuardianAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _guardianRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Guardian);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _guardianService.DeleteGuardianAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucun responsable correspondant n'a été trouvé.");

            _guardianRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}