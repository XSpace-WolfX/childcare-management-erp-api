using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Infrastructure.Persistence.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class PersonalSituationServiceTests
    {
        private readonly IPersonalSituationService _personalSituationService;
        private readonly Mock<IPersonalSituationRepository> _personalSituationRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public PersonalSituationServiceTests()
        {
            _personalSituationRepositoryMock = new Mock<IPersonalSituationRepository>();
            _mapperMock = new Mock<IMapper>();
            _personalSituationService = new PersonalSituationService(_personalSituationRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllPersonalSituationsAsync_WhenPersonalSituationsExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var personalSituations = new List<PersonalSituation>
            {
                new() { Id = 1, MaritalStatus = "Situation 1" },
                new() { Id = 2, MaritalStatus = "Situation 2" }
            };

            var personalSituationsDto = new List<PersonalSituationDto>
            {
                new() { Id = 1, MaritalStatus = "Situation 1" },
                new() { Id = 2, MaritalStatus = "Situation 2" }
            };

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(personalSituations);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<PersonalSituationDto>>(personalSituations))
                .Returns(personalSituationsDto);

            // Act
            var result = await _personalSituationService.GetAllPersonalSituationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(x => x.Id == 1 && x.MaritalStatus == "Situation 1");
        }

        [Fact]
        public async Task GetAllPersonalSituationsAsync_WhenNoPersonalSituation_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var personalSituations = new List<PersonalSituation>();
            var personalSituationsDto = new List<PersonalSituationDto>();

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(personalSituations);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<PersonalSituationDto>>(personalSituations))
                .Returns(personalSituationsDto);

            // Act
            var result = await _personalSituationService.GetAllPersonalSituationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonalSituationAsync_WhenPersonalSituationExists_ShouldReturnMappedDto()
        {
            // Arrange
            var personalSituation = new PersonalSituation { Id = 1, MaritalStatus = "Situation 1" };
            var personalSituationDto = new PersonalSituationDto { Id = 1, MaritalStatus = "Situation 1" };

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(personalSituation);

            _mapperMock
                .Setup(mapper => mapper.Map<PersonalSituationDto>(personalSituation))
                .Returns(personalSituationDto);

            // Act
            var result = await _personalSituationService.GetPersonalSituationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.MaritalStatus.ShouldBe("Situation 1");
        }

        [Fact]
        public async Task GetPersonalSituationAsync_WhenPersonalSituationDoesNotExist_ShouldReturnFailResult()
        {
            // Arrange
            _personalSituationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as PersonalSituation);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personalSituationService.GetPersonalSituationAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreatePersonalSituationAsync_WhenPersonalSituationIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newPersonalSituationDto = new CreatePersonalSituationDto { MaritalStatus = "Situation 1" };
            var personalSituation = new PersonalSituation { Id = 1, MaritalStatus = "Situation 1" };
            var createdPersonalSituationDto = new PersonalSituationDto { Id = 1, MaritalStatus = "Situation 1" };

            _mapperMock
                .Setup(mapper => mapper.Map<PersonalSituation>(newPersonalSituationDto))
                .Returns(personalSituation);

            _personalSituationRepositoryMock
                .Setup(repo => repo.AddAsync(personalSituation))
                .Returns(Task.CompletedTask);

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(personalSituation.Id))
                .ReturnsAsync(personalSituation);

            _mapperMock
                .Setup(mapper => mapper.Map<PersonalSituationDto>(personalSituation))
                .Returns(createdPersonalSituationDto);

            // Act
            var result = await _personalSituationService.CreatePersonalSituationAsync(newPersonalSituationDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.MaritalStatus.ShouldBe("Situation 1");

            _personalSituationRepositoryMock.Verify(repo => repo.AddAsync(personalSituation), Times.Once);
        }

        [Fact]
        public async Task CreatePersonalSituationAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newPersonalSituationDto = new CreatePersonalSituationDto { MaritalStatus = "Situation 1" };

            _mapperMock
                .Setup(mapper => mapper.Map<PersonalSituation>(newPersonalSituationDto))
                .Returns((PersonalSituation)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personalSituationService.CreatePersonalSituationAsync(newPersonalSituationDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de la situation personnelle : Le Mapping a échoué.");

            _personalSituationRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<PersonalSituation>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePersonalSituationAsync_WhenPersonalSituationExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var personalSituation = new PersonalSituation { Id = id, MaritalStatus = "Situation 1" };
            var updatePersonalSituationDto = new UpdatePersonalSituationDto { Id = 1, MaritalStatus = "Updated Situation" };

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(personalSituation);

            _mapperMock
                .Setup(mapper => mapper.Map(updatePersonalSituationDto, personalSituation))
                .Returns(personalSituation);

            _personalSituationRepositoryMock
                .Setup(repo => repo.UpdateAsync(personalSituation))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _personalSituationService.UpdatePersonalSituationAsync(1, updatePersonalSituationDto));

            _personalSituationRepositoryMock.Verify(repo => repo.UpdateAsync(personalSituation), Times.Once);
        }

        [Fact]
        public async Task UpdatePersonalSituationAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updatePersonalSituationDto = new UpdatePersonalSituationDto { Id = 2, MaritalStatus = "Updated Situation" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personalSituationService.UpdatePersonalSituationAsync(id, updatePersonalSituationDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de la situation personnelle ne correspond pas à celui de l'objet envoyé.");

            _personalSituationRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<PersonalSituation>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePersonalSituationAsync_WhenPersonalSituationDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updatePersonalSituationDto = new UpdatePersonalSituationDto { Id = id, MaritalStatus = "Updated Situation" };

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as PersonalSituation);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personalSituationService.UpdatePersonalSituationAsync(id, updatePersonalSituationDto));

            // Assert
            exception.Message.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");

            _personalSituationRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<PersonalSituation>()), Times.Never);
        }

        [Fact]
        public async Task DeletePersonalSituationAsync_WhenPersonalSituationExists_ShouldReturnTrue()
        {
            // Arrange
            var id = 1;
            var personalSituation = new PersonalSituation { Id = id, MaritalStatus = "Situation 1" };

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(personalSituation);

            _personalSituationRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _personalSituationService.DeletePersonalSituationAsync(id));

            _personalSituationRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeletePersonalSituationAsync_WhenPersonalSituationDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _personalSituationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as PersonalSituation);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personalSituationService.DeletePersonalSituationAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");

            _personalSituationRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}