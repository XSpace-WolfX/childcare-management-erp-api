using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;
using ChildcareManagementERP.Api.Services;
using Moq;
using Shouldly;

namespace ChildcareManagementERP.Api.UnitTests.Services
{
    public class AdditionalDataServiceTests
    {
        private readonly IAdditionalDataService _additionalDataService;
        private readonly Mock<IAdditionalDataRepository> _additionalDataRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public AdditionalDataServiceTests()
        {
            _additionalDataRepositoryMock = new Mock<IAdditionalDataRepository>();
            _mapperMock = new Mock<IMapper>();
            _additionalDataService = new AdditionalDataService(_additionalDataRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAdditionalDatasAsync_WhenAdditionalDatasExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var additionalDatas = new List<AdditionalDatum>
            {
                new() { Id = 1, ChildId = 3, ParamName = "Allergie" },
                new() { Id = 2, ChildId = 7, ParamName = "Scolarisé", ParamType = "bool" }
            };

            var additionalDatasDtos = new List<AdditionalDataDto>
            {
                new() { Id = 1, ChildId = 3, ParamName = "Allergie" },
                new() { Id = 2, ChildId = 7, ParamName = "Scolarisé", ParamType = "bool" }
            };

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(additionalDatas);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AdditionalDataDto>>(additionalDatas))
                .Returns(additionalDatasDtos);

            // Act
            var result = await _additionalDataService.GetAllAdditionalDatasAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(r => r.ParamName == "Allergie");
        }

        [Fact]
        public async Task GetAllAdditionalDatasAsync_WhenNoAdditionalData_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var additionalDatas = new List<AdditionalDatum>();
            var additionalDatasDtos = new List<AdditionalDataDto>();

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(additionalDatas);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AdditionalDataDto>>(additionalDatas))
                .Returns(additionalDatasDtos);

            // Act
            var result = await _additionalDataService.GetAllAdditionalDatasAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAdditionalDataAsync_WhenAdditionalDataExists_ShouldReturnMappedDto()
        {
            // Arrange
            var additionalData = new AdditionalDatum { Id = 1, ChildId = 3, ParamName = "Allergie" };
            var additionalDataDto = new AdditionalDataDto { Id = 1, ChildId = 3, ParamName = "Allergie" };

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(additionalData);

            _mapperMock
                .Setup(m => m.Map<AdditionalDataDto>(additionalData))
                .Returns(additionalDataDto);

            // Act
            var result = await _additionalDataService.GetAdditionalDataAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ParamName.ShouldBe("Allergie");
        }

        [Fact]
        public async Task GetAdditionalDataAsync_WhenAdditionalDataDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _additionalDataRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as AdditionalDatum);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _additionalDataService.GetAdditionalDataAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateAdditionalDataAsync_WhenAdditionalDataIsCreated_ShouldReturnCreatedDto()
        {
            // Arrange
            var newAdditionalDataDto = new CreateAdditionalDataDto { ChildId = 3, ParamName = "Allergie" };
            var additionalData = new AdditionalDatum { Id = 1, ChildId = 3, ParamName = "Allergie" };
            var createdDonneeSupplementaireDto = new AdditionalDataDto { Id = 1, ChildId = 3, ParamName = "Allergie" };

            _mapperMock
                .Setup(m => m.Map<AdditionalDatum>(newAdditionalDataDto))
                .Returns(additionalData);

            _additionalDataRepositoryMock
                .Setup(repo => repo.AddAsync(additionalData))
                .Returns(Task.CompletedTask);

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetByIdAsync(additionalData.Id))
                .ReturnsAsync(additionalData);

            _mapperMock
                .Setup(m => m.Map<AdditionalDataDto>(additionalData))
                .Returns(createdDonneeSupplementaireDto);

            // Act
            var result = await _additionalDataService.CreateAdditionalDataAsync(newAdditionalDataDto);

            // Assert
            result.ShouldNotBeNull();
            result.ParamName.ShouldBe("Allergie");
        }

        [Fact]
        public async Task CreateAdditionalDataAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newAdditionalDataDto = new CreateAdditionalDataDto { ChildId = 3, ParamName = "Allergie" };

            _mapperMock
                .Setup(m => m.Map<AdditionalDatum>(newAdditionalDataDto))
                .Returns((AdditionalDatum)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _additionalDataService.CreateAdditionalDataAsync(newAdditionalDataDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de la donnée supplémentaire : Le Mapping a échoué.");

            _additionalDataRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<AdditionalDatum>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAdditionalDataAsync_WhenAdditionalDataExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var additionalData = new AdditionalDatum { Id = id, ChildId = 3, ParamName = "Scolarisé" };
            var updateAdditionalDataDto = new UpdateAdditionalDataDto { Id = 1, ChildId = 3, ParamName = "Scolarisé" };

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(additionalData);

            _mapperMock
                .Setup(m => m.Map(updateAdditionalDataDto, additionalData))
                .Returns(additionalData);

            _additionalDataRepositoryMock
                .Setup(repo => repo.UpdateAsync(additionalData))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _additionalDataService.UpdateAdditionalDataAsync(id, updateAdditionalDataDto));

            _additionalDataRepositoryMock.Verify(r => r.UpdateAsync(additionalData), Times.Once);
        }

        [Fact]
        public async Task UpdateAdditionalDataAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateAdditionalDataDto = new UpdateAdditionalDataDto { Id = 6, ChildId = 3, ParamName = "Scolarisé" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _additionalDataService.UpdateAdditionalDataAsync(id, updateAdditionalDataDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de la donnée supplémentaire ne correspond pas à celui de l'objet envoyé.");

            _additionalDataRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AdditionalDatum>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAdditionalDataAsync_WhenAdditionalDataDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateAdditionalDataDto = new UpdateAdditionalDataDto { Id = id, ChildId = 3, ParamName = "Scolarisé" };

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as AdditionalDatum);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _additionalDataService.UpdateAdditionalDataAsync(id, updateAdditionalDataDto));

            // Assert
            exception.Message.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            _additionalDataRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<AdditionalDatum>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAdditionalDataAsync_WhenAdditionalDataExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var additionalData = new AdditionalDatum { Id = id, ChildId = 3, ParamName = "Scolarisé" };

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(additionalData);

            _additionalDataRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _additionalDataService.DeleteAdditionalDataAsync(id));

            _additionalDataRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteAdditionalDataAsync_WhenAdditionalDataDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _additionalDataRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as AdditionalDatum);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _additionalDataService.DeleteAdditionalDataAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            _additionalDataRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}