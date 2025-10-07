using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class InformationFinanciereServiceTests
    {
        private readonly IFinancialInformationService _informationFinanciereService;
        private readonly Mock<IFinancialInformationRepository> _informationFinanciereRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public InformationFinanciereServiceTests()
        {
            _informationFinanciereRepositoryMock = new Mock<IFinancialInformationRepository>();
            _mapperMock = new Mock<IMapper>();
            _informationFinanciereService = new FinancialInformationService(_informationFinanciereRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllInformationsFinancieresAsync_WhenInformationsFinancieresExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var informationsFinancieres = new List<InformationFinanciere>
            {
                new() { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) },
                new() { Id = 2, ResponsableId = 200, DateDebut = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) }
            };

            var informationsFinancieresDto = new List<FinancialInformationDto>
            {
                new() { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) },
                new() { Id = 2, GuardianId = 200, StartDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) }
            };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(informationsFinancieres);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<FinancialInformationDto>>(informationsFinancieres))
                .Returns(informationsFinancieresDto);

            // Act
            var result = await _informationFinanciereService.GetAllFinancialInformationsAsync();

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Count().ShouldBe(2);
            result.Data.ShouldContain(e => e.ResponsableId == 100);
        }

        [Fact]
        public async Task GetAllInformationsFinancieresAsync_WhenNoInformationsFinancieres_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var informationsFinancieres = new List<InformationFinanciere>();
            var informationsFinancieresDto = new List<FinancialInformationDto>();

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(informationsFinancieres);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<FinancialInformationDto>>(informationsFinancieres))
                .Returns(informationsFinancieresDto);

            // Act
            var result = await _informationFinanciereService.GetAllFinancialInformationsAsync();

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetInformationFinanciereAsync_WhenInformationFinanciereExists_ShouldReturnMappedDto()
        {
            // Arrange
            var informationFinanciere = new InformationFinanciere { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };
            var informationFinanciereDto = new FinancialInformationDto { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(informationFinanciere);

            _mapperMock
                .Setup(m => m.Map<FinancialInformationDto>(informationFinanciere))
                .Returns(informationFinanciereDto);

            // Act
            var result = await _informationFinanciereService.GetFinancialInformationAsync(1);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ResponsableId.ShouldBe(100);
        }

        [Fact]
        public async Task GetInformationFinanciereAsync_WhenInformationFinanciereDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as InformationFinanciere);

            // Act
            var result = await _informationFinanciereService.GetFinancialInformationAsync(1);

            // Assert
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée");
        }

        [Fact]
        public async Task CreateInformationFinanciereAsync_WhenInformationFinanciereIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newInformationFinanciereDto = new CreateFinancialInformationDto { GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };
            var informationFinanciere = new InformationFinanciere { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };
            var createdInformationFinanciereDto = new FinancialInformationDto { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _mapperMock
                .Setup(m => m.Map<InformationFinanciere>(newInformationFinanciereDto))
                .Returns(informationFinanciere);

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.AddAsync(informationFinanciere))
                .Returns(Task.CompletedTask);

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(informationFinanciere.Id))
                .ReturnsAsync(informationFinanciere);

            _mapperMock
                .Setup(m => m.Map<FinancialInformationDto>(informationFinanciere))
                .Returns(createdInformationFinanciereDto);

            // Act
            var result = await _informationFinanciereService.CreateFinancialInformationAsync(newInformationFinanciereDto);

            // Assert
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.ResponsableId.ShouldBe(100);

            _informationFinanciereRepositoryMock.Verify(repo => repo.AddAsync(informationFinanciere), Times.Once);
        }

        [Fact]
        public async Task CreateInformationFinanciereAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newInformationFinanciereDto = new CreateFinancialInformationDto { GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _mapperMock
                .Setup(m => m.Map<InformationFinanciere>(newInformationFinanciereDto))
                .Returns((InformationFinanciere)null!);

            // Act
            var result = await _informationFinanciereService.CreateFinancialInformationAsync(newInformationFinanciereDto);

            // Assert
            result.Success.ShouldBeFalse();
            result.Data.ShouldBeNull();
            result.Message.ShouldBe("Erreur lors de la création de l'information financière : Le Mapping a échoué");

            _informationFinanciereRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<InformationFinanciere>()), Times.Never);
        }

        [Fact]
        public async Task UpdateInformationFinanciereAsync_WhenEnfantExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var informationFinanciere = new InformationFinanciere { Id = id, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };
            var updateInformationFinanciereDto = new UpdateFinancialInformationDto { Id = 1, GuardianId = 200, StartDate = new DateOnly(DateTime.Now.Year, 2, 1) };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(informationFinanciere);

            _mapperMock
                .Setup(m => m.Map(updateInformationFinanciereDto, informationFinanciere))
                .Returns(informationFinanciere);

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.UpdateAsync(informationFinanciere))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _informationFinanciereService.UpdateFinancialInformationAsync(id, updateInformationFinanciereDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            _informationFinanciereRepositoryMock.Verify(r => r.UpdateAsync(informationFinanciere), Times.Once);
        }

        [Fact]
        public async Task UpdateInformationFinanciereAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateInformationFinanciereDto = new UpdateFinancialInformationDto { Id = 3, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            // Act
            var result = await _informationFinanciereService.UpdateFinancialInformationAsync(id, updateInformationFinanciereDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("L'identifiant de l'information financière ne correspond pas à celui de l'objet envoyé");

            _informationFinanciereRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InformationFinanciere>()), Times.Never);
        }

        [Fact]
        public async Task UpdateInformationFinanciereAsync_WhenInformationFinanciereDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateInformationFinanciereDto = new UpdateFinancialInformationDto { Id = id, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as InformationFinanciere);

            // Act
            var result = await _informationFinanciereService.UpdateFinancialInformationAsync(id, updateInformationFinanciereDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée");

            _informationFinanciereRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InformationFinanciere>()), Times.Never);
        }

        [Fact]
        public async Task DeleteInformationFinanciereAsync_WhenInformationFinanciereExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var informationFinanciere = new InformationFinanciere { Id = id, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(informationFinanciere);

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _informationFinanciereService.DeleteFinancialInformationAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();

            _informationFinanciereRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteInformationFinanciereAsync_WhenInformationFinanciereDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as InformationFinanciere);

            // Act
            var result = await _informationFinanciereService.DeleteFinancialInformationAsync(id);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée");

            _informationFinanciereRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}