using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class DonneeSupplementaireServiceTests
    {
        private readonly IAdditionalDataService _donneeSupplementaireService;
        private readonly Mock<IAdditionalDataRepository> _donneeSupplementaireRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public DonneeSupplementaireServiceTests()
        {
            _donneeSupplementaireRepositoryMock = new Mock<IAdditionalDataRepository>();
            _mapperMock = new Mock<IMapper>();
            _donneeSupplementaireService = new AdditionalDataService(_donneeSupplementaireRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllDonneesSupplementairesAsync_WhenDonneesSupplementairesExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var donneesSupplementaires = new List<DonneeSupplementaire>
            {
                new() { Id = 1, EnfantId = 3, Parametre = "Allergie" },
                new() { Id = 2, EnfantId = 7, Parametre = "Scolarisé", Type = "bool" }
            };

            var donneesSupplementairesDtos = new List<AdditionalDataDto>
            {
                new() { Id = 1, ChildId = 3, ParamName = "Allergie" },
                new() { Id = 2, ChildId = 7, ParamName = "Scolarisé", ParamType = "bool" }
            };

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(donneesSupplementaires);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AdditionalDataDto>>(donneesSupplementaires))
                .Returns(donneesSupplementairesDtos);

            // Act
            var result = await _donneeSupplementaireService.GetAllAdditionalDatasAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.ParamName == "Allergie");
        }

        [Fact]
        public async Task GetAllDonneesSupplementairesAsync_WhenNoDonneesSupplementaires_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var donneesSupplementaires = new List<DonneeSupplementaire>();
            var donneesSupplementairesDtos = new List<AdditionalDataDto>();

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(donneesSupplementaires);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AdditionalDataDto>>(donneesSupplementaires))
                .Returns(donneesSupplementairesDtos);

            // Act
            var result = await _donneeSupplementaireService.GetAllAdditionalDatasAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetDonneeSupplementaireAsync_WhenDonneeSupplementaireExists_ShouldReturnMappedDto()
        {
            // Arrange
            var donneeSupplementaire = new DonneeSupplementaire { Id = 1, EnfantId = 3, Parametre = "Allergie" };
            var donneeSupplementaireDto = new AdditionalDataDto { Id = 1, ChildId = 3, ParamName = "Allergie" };

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(donneeSupplementaire);

            _mapperMock
                .Setup(m => m.Map<AdditionalDataDto>(donneeSupplementaire))
                .Returns(donneeSupplementaireDto);

            // Act
            var result = await _donneeSupplementaireService.GetAdditionalDataAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ParamName.ShouldBe("Allergie");
        }

        [Fact]
        public async Task GetDonneeSupplementaireAsync_WhenDonneeSupplementaireDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as DonneeSupplementaire);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _donneeSupplementaireService.GetAdditionalDataAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateDonneeSupplementaireAsync_WhenDonneeSupplementaireIsCreated_ShouldReturnCreatedDto()
        {
            // Arrange
            var newDonneeSupplementaireDto = new CreateAdditionalDataDto { ChildId = 3, ParamName = "Allergie" };
            var donneeSupplementaire = new DonneeSupplementaire { Id = 1, EnfantId = 3, Parametre = "Allergie" };
            var createdDonneeSupplementaireDto = new AdditionalDataDto { Id = 1, ChildId = 3, ParamName = "Allergie" };

            _mapperMock
                .Setup(m => m.Map<DonneeSupplementaire>(newDonneeSupplementaireDto))
                .Returns(donneeSupplementaire);

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.AddAsync(donneeSupplementaire))
                .Returns(Task.CompletedTask);

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetByIdAsync(donneeSupplementaire.Id))
                .ReturnsAsync(donneeSupplementaire);

            _mapperMock
                .Setup(m => m.Map<AdditionalDataDto>(donneeSupplementaire))
                .Returns(createdDonneeSupplementaireDto);

            // Act
            var result = await _donneeSupplementaireService.CreateAdditionalDataAsync(newDonneeSupplementaireDto);

            // Assert
            result.ShouldNotBeNull();
            result.ParamName.ShouldBe("Allergie");
        }

        [Fact]
        public async Task CreateDonneeSupplementaireAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newDonneeSupplementaireDto = new CreateAdditionalDataDto { ChildId = 3, ParamName = "Allergie" };

            _mapperMock
                .Setup(m => m.Map<DonneeSupplementaire>(newDonneeSupplementaireDto))
                .Returns((DonneeSupplementaire)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _donneeSupplementaireService.CreateAdditionalDataAsync(newDonneeSupplementaireDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de la donnée supplémentaire : Le Mapping a échoué.");

            _donneeSupplementaireRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<DonneeSupplementaire>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaireAsync_WhenDonneSupplementaireExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var donneeSupplementaire = new DonneeSupplementaire { Id = id, EnfantId = 3, Parametre = "Scolarisé" };
            var updateDonneeSupplementaireDto = new UpdateAdditionalDataDto { Id = 1, ChildId = 3, ParamName = "Scolarisé" };

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(donneeSupplementaire);

            _mapperMock
                .Setup(m => m.Map(updateDonneeSupplementaireDto, donneeSupplementaire))
                .Returns(donneeSupplementaire);

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.UpdateAsync(donneeSupplementaire))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _donneeSupplementaireService.UpdateAdditionalDataAsync(id, updateDonneeSupplementaireDto));

            _donneeSupplementaireRepositoryMock.Verify(r => r.UpdateAsync(donneeSupplementaire), Times.Once);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaireAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateDonneeSupplementaireDto = new UpdateAdditionalDataDto { Id = 6, ChildId = 3, ParamName = "Scolarisé" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _donneeSupplementaireService.UpdateAdditionalDataAsync(id, updateDonneeSupplementaireDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de la donnée supplémentaire ne correspond pas à celui de l'objet envoyé.");

            _donneeSupplementaireRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<DonneeSupplementaire>()), Times.Never);
        }

        [Fact]
        public async Task UpdateDonneeSupplementaireAsync_WhenDonneeSupplementaireDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateDonneeSupplementaireDto = new UpdateAdditionalDataDto { Id = id, ChildId = 3, ParamName = "Scolarisé" };

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as DonneeSupplementaire);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _donneeSupplementaireService.UpdateAdditionalDataAsync(id, updateDonneeSupplementaireDto));

            // Assert
            exception.Message.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            _donneeSupplementaireRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<DonneeSupplementaire>()), Times.Never);
        }

        [Fact]
        public async Task DeleteDonneeSupplementaireAsync_WhenDonneeSupplementaireExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var donneeSupplementaire = new DonneeSupplementaire { Id = id, EnfantId = 3, Parametre = "Scolarisé" };

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(donneeSupplementaire);

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _donneeSupplementaireService.DeleteAdditionalDataAsync(id));

            _donneeSupplementaireRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteDonneeSupplementaireAsync_WhenDonneeSupplementaireDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _donneeSupplementaireRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as DonneeSupplementaire);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _donneeSupplementaireService.DeleteAdditionalDataAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune donnée supplémentaire correspondante n'a été trouvée.");

            _donneeSupplementaireRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}