using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class SituationPersonnelleServiceTests
    {
        private readonly IPersonalSituationService _situationPersonnelleService;
        private readonly Mock<IPersonalSituationRepository> _situationPersonnelleRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public SituationPersonnelleServiceTests()
        {
            _situationPersonnelleRepositoryMock = new Mock<IPersonalSituationRepository>();
            _mapperMock = new Mock<IMapper>();
            _situationPersonnelleService = new PersonalSituationService(_situationPersonnelleRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllSituationsPersonnellesAsync_WhenSituationsPersonnellesExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var situationPersonnelles = new List<SituationPersonnelle>
            {
                new() { Id = 1, SituationFamiliale = "Situation 1" },
                new() { Id = 2, SituationFamiliale = "Situation 2" }
            };

            var situationPersonnelleDtos = new List<PersonalSituationDto>
            {
                new() { Id = 1, FamilySituation = "Situation 1" },
                new() { Id = 2, FamilySituation = "Situation 2" }
            };

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(situationPersonnelles);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<PersonalSituationDto>>(situationPersonnelles))
                .Returns(situationPersonnelleDtos);

            // Act
            var result = await _situationPersonnelleService.GetAllPersonalSituationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(x => x.Id == 1 && x.FamilySituation == "Situation 1");
        }

        [Fact]
        public async Task GetAllSituationsPersonnellesAsync_WhenNoSituationsPersonnelles_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var situationPersonnelles = new List<SituationPersonnelle>();
            var situationPersonnelleDtos = new List<PersonalSituationDto>();

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(situationPersonnelles);

            _mapperMock
                .Setup(mapper => mapper.Map<IEnumerable<PersonalSituationDto>>(situationPersonnelles))
                .Returns(situationPersonnelleDtos);

            // Act
            var result = await _situationPersonnelleService.GetAllPersonalSituationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetSituationPersonnelleAsync_WhenSituationPersonnelleExists_ShouldReturnMappedDto()
        {
            // Arrange
            var situationPersonnelle = new SituationPersonnelle { Id = 1, SituationFamiliale = "Situation 1" };
            var situationPersonnelleDto = new PersonalSituationDto { Id = 1, FamilySituation = "Situation 1" };

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(situationPersonnelle);

            _mapperMock
                .Setup(mapper => mapper.Map<PersonalSituationDto>(situationPersonnelle))
                .Returns(situationPersonnelleDto);

            // Act
            var result = await _situationPersonnelleService.GetPersonalSituationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FamilySituation.ShouldBe("Situation 1");
        }

        [Fact]
        public async Task GetSituationPersonnelleAsync_WhenSituationPersonnelleDoesNotExist_ShouldReturnFailResult()
        {
            // Arrange
            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as SituationPersonnelle);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _situationPersonnelleService.GetPersonalSituationAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateSituationPersonnelleAsync_WhenSituationPersonnelleIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newCreateDto = new CreatePersonalSituationDto { FamilySituation = "Situation 1" };
            var situationPersonnelle = new SituationPersonnelle { Id = 1, SituationFamiliale = "Situation 1" };
            var situationPersonnelleDtoCreated = new PersonalSituationDto { Id = 1, FamilySituation = "Situation 1" };

            _mapperMock
                .Setup(mapper => mapper.Map<SituationPersonnelle>(newCreateDto))
                .Returns(situationPersonnelle);

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.AddAsync(situationPersonnelle))
                .Returns(Task.CompletedTask);

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(situationPersonnelle.Id))
                .ReturnsAsync(situationPersonnelle);

            _mapperMock
                .Setup(mapper => mapper.Map<PersonalSituationDto>(situationPersonnelle))
                .Returns(situationPersonnelleDtoCreated);

            // Act
            var result = await _situationPersonnelleService.CreatePersonalSituationAsync(newCreateDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FamilySituation.ShouldBe("Situation 1");

            _situationPersonnelleRepositoryMock.Verify(repo => repo.AddAsync(situationPersonnelle), Times.Once);
        }

        [Fact]
        public async Task CreateSituationPersonnelleAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newCreateDto = new CreatePersonalSituationDto { FamilySituation = "Situation 1" };

            _mapperMock
                .Setup(mapper => mapper.Map<SituationPersonnelle>(newCreateDto))
                .Returns((SituationPersonnelle)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _situationPersonnelleService.CreatePersonalSituationAsync(newCreateDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de la situation personnelle : Le Mapping a échoué.");

            _situationPersonnelleRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<SituationPersonnelle>()), Times.Never);
        }

        [Fact]
        public async Task UpdateSituationPersonnelleAsync_WhenSituationPersonnelleExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var situationPersonnelle = new SituationPersonnelle { Id = id, SituationFamiliale = "Situation 1" };
            var updateSituationPersonnelleDto = new UpdatePersonalSituationDto { Id = 1, FamilySituation = "Updated Situation" };

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(situationPersonnelle);

            _mapperMock
                .Setup(mapper => mapper.Map(updateSituationPersonnelleDto, situationPersonnelle))
                .Returns(situationPersonnelle);

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.UpdateAsync(situationPersonnelle))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _situationPersonnelleService.UpdatePersonalSituationAsync(1, updateSituationPersonnelleDto));

            _situationPersonnelleRepositoryMock.Verify(repo => repo.UpdateAsync(situationPersonnelle), Times.Once);
        }

        [Fact]
        public async Task UpdateSituationPersonnelleAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateSituationPersonnelleDto = new UpdatePersonalSituationDto { Id = 2, FamilySituation = "Updated Situation" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _situationPersonnelleService.UpdatePersonalSituationAsync(id, updateSituationPersonnelleDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de la situation personnelle ne correspond pas à celui de l'objet envoyé.");

            _situationPersonnelleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SituationPersonnelle>()), Times.Never);
        }

        [Fact]
        public async Task UpdateSituationPersonnelleAsync_WhenSituationPersonnelleDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateSituationPersonnelleDto = new UpdatePersonalSituationDto { Id = id, FamilySituation = "Updated Situation" };

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as SituationPersonnelle);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _situationPersonnelleService.UpdatePersonalSituationAsync(id, updateSituationPersonnelleDto));

            // Assert
            exception.Message.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");

            _situationPersonnelleRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<SituationPersonnelle>()), Times.Never);
        }

        [Fact]
        public async Task DeleteSituationPersonnelleAsync_WhenSituationPersonnelleExists_ShouldReturnTrue()
        {
            // Arrange
            var id = 1;
            var situationPersonnelle = new SituationPersonnelle { Id = id, SituationFamiliale = "Situation 1" };

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(situationPersonnelle);

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _situationPersonnelleService.DeletePersonalSituationAsync(id));

            _situationPersonnelleRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteSituationPersonnelleAsync_WhenSituationPersonnelleDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _situationPersonnelleRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as SituationPersonnelle);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _situationPersonnelleService.DeletePersonalSituationAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune situation personnelle correspondante n'a été trouvée.");

            _situationPersonnelleRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}