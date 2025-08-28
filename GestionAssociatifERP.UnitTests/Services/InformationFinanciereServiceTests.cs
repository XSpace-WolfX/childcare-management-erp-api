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
        private readonly IInformationFinanciereService _informationFinanciereService;
        private readonly Mock<IInformationFinanciereRepository> _informationFinanciereRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public InformationFinanciereServiceTests()
        {
            _informationFinanciereRepositoryMock = new Mock<IInformationFinanciereRepository>();
            _mapperMock = new Mock<IMapper>();
            _informationFinanciereService = new InformationFinanciereService(_informationFinanciereRepositoryMock.Object, _mapperMock.Object);
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

            var informationsFinancieresDto = new List<InformationFinanciereDto>
            {
                new() { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) },
                new() { Id = 2, ResponsableId = 200, DateDebut = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) }
            };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(informationsFinancieres);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<InformationFinanciereDto>>(informationsFinancieres))
                .Returns(informationsFinancieresDto);

            // Act
            var result = await _informationFinanciereService.GetAllInformationsFinancieresAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.ResponsableId == 100);
        }

        [Fact]
        public async Task GetAllInformationsFinancieresAsync_WhenNoInformationsFinancieres_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var informationsFinancieres = new List<InformationFinanciere>();
            var informationsFinancieresDto = new List<InformationFinanciereDto>();

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(informationsFinancieres);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<InformationFinanciereDto>>(informationsFinancieres))
                .Returns(informationsFinancieresDto);

            // Act
            var result = await _informationFinanciereService.GetAllInformationsFinancieresAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetInformationFinanciereAsync_WhenInformationFinanciereExists_ShouldReturnMappedDto()
        {
            // Arrange
            var informationFinanciere = new InformationFinanciere { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };
            var informationFinanciereDto = new InformationFinanciereDto { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(informationFinanciere);

            _mapperMock
                .Setup(m => m.Map<InformationFinanciereDto>(informationFinanciere))
                .Returns(informationFinanciereDto);

            // Act
            var result = await _informationFinanciereService.GetInformationFinanciereAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ResponsableId.ShouldBe(100);
        }

        [Fact]
        public async Task GetInformationFinanciereAsync_WhenInformationFinanciereDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as InformationFinanciere);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _informationFinanciereService.GetInformationFinanciereAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateInformationFinanciereAsync_WhenInformationFinanciereIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newInformationFinanciereDto = new CreateInformationFinanciereDto { ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };
            var informationFinanciere = new InformationFinanciere { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };
            var createdInformationFinanciereDto = new InformationFinanciereDto { Id = 1, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };

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
                .Setup(m => m.Map<InformationFinanciereDto>(informationFinanciere))
                .Returns(createdInformationFinanciereDto);

            // Act
            var result = await _informationFinanciereService.CreateInformationFinanciereAsync(newInformationFinanciereDto);

            // Assert
            result.ShouldNotBeNull();
            result.ResponsableId.ShouldBe(100);

            _informationFinanciereRepositoryMock.Verify(repo => repo.AddAsync(informationFinanciere), Times.Once);
        }

        [Fact]
        public async Task CreateInformationFinanciereAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newInformationFinanciereDto = new CreateInformationFinanciereDto { ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };

            _mapperMock
                .Setup(m => m.Map<InformationFinanciere>(newInformationFinanciereDto))
                .Returns((InformationFinanciere)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _informationFinanciereService.CreateInformationFinanciereAsync(newInformationFinanciereDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de l'information financière : Le Mapping a échoué.");

            _informationFinanciereRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<InformationFinanciere>()), Times.Never);
        }

        [Fact]
        public async Task UpdateInformationFinanciereAsync_WhenEnfantExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var informationFinanciere = new InformationFinanciere { Id = id, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };
            var updateInformationFinanciereDto = new UpdateInformationFinanciereDto { Id = 1, ResponsableId = 200, DateDebut = new DateOnly(DateTime.Now.Year, 2, 1) };

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

            // Assert
            await Should.NotThrowAsync(async () => await _informationFinanciereService.UpdateInformationFinanciereAsync(id, updateInformationFinanciereDto));

            _informationFinanciereRepositoryMock.Verify(r => r.UpdateAsync(informationFinanciere), Times.Once);
        }

        [Fact]
        public async Task UpdateInformationFinanciereAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateInformationFinanciereDto = new UpdateInformationFinanciereDto { Id = 3, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _informationFinanciereService.UpdateInformationFinanciereAsync(id, updateInformationFinanciereDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de l'information financière ne correspond pas à celui de l'objet envoyé.");

            _informationFinanciereRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<InformationFinanciere>()), Times.Never);
        }

        [Fact]
        public async Task UpdateInformationFinanciereAsync_WhenInformationFinanciereDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateInformationFinanciereDto = new UpdateInformationFinanciereDto { Id = id, ResponsableId = 100, DateDebut = new DateOnly(DateTime.Now.Year, 1, 1) };

            _informationFinanciereRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as InformationFinanciere);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _informationFinanciereService.UpdateInformationFinanciereAsync(id, updateInformationFinanciereDto));

            // Assert
            exception.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée.");

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

            // Assert
            await Should.NotThrowAsync(async () => await _informationFinanciereService.DeleteInformationFinanciereAsync(id));

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
            var exception = await Should.ThrowAsync<Exception>(async () => await _informationFinanciereService.DeleteInformationFinanciereAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée.");

            _informationFinanciereRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}