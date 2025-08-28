using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;
using System.Linq.Expressions;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class LinkResponsableEnfantServiceTests
    {
        private readonly ILinkResponsableEnfantService _linkResponsableEnfantService;
        private readonly Mock<IResponsableEnfantRepository> _responsableEnfantRepositoryMock;
        private readonly Mock<IEnfantRepository> _enfantRepositoryMock;
        private readonly Mock<IResponsableRepository> _responsableRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public LinkResponsableEnfantServiceTests()
        {
            _responsableEnfantRepositoryMock = new Mock<IResponsableEnfantRepository>();
            _enfantRepositoryMock = new Mock<IEnfantRepository>();
            _responsableRepositoryMock = new Mock<IResponsableRepository>();
            _mapperMock = new Mock<IMapper>();

            _linkResponsableEnfantService = new LinkResponsableEnfantService(
                _responsableEnfantRepositoryMock.Object,
                _enfantRepositoryMock.Object,
                _responsableRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetResponsablesByEnfantIdAsync_WhenEnfantExist_ShouldReturnDtoList()
        {
            // Arrange
            var links = new List<ResponsableEnfant>
            {
                new() { EnfantId = 1, ResponsableId = 1, Affiliation = "Père" }
            };

            var dtos = new List<LinkResponsableEnfantDto>
            {
                new() { EnfantId = 1, ResponsableId = 1, Affiliation = "Père" }
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetResponsablesByEnfantIdAsync(1))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkResponsableEnfantDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkResponsableEnfantService.GetResponsablesByEnfantIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldHaveSingleItem();
            result.First().Affiliation.ShouldBe("Père");
        }

        [Fact]
        public async Task GetResponsablesByEnfantIdAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.GetResponsablesByEnfantIdAsync(1));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetResponsablesByEnfantIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetResponsablesByEnfantIdAsync(1))
                .ReturnsAsync(new List<ResponsableEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkResponsableEnfantDto>>(It.IsAny<IEnumerable<ResponsableEnfant>>()))
                .Returns(new List<LinkResponsableEnfantDto>());

            // Act
            var result = await _linkResponsableEnfantService.GetResponsablesByEnfantIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantsByResponsableIdAsync_WhenResponsableExist_ShouldReturnDtoList()
        {
            // Arrange
            var links = new List<ResponsableEnfant>
            {
                new ResponsableEnfant { EnfantId = 1, ResponsableId = 2, Affiliation = "Tuteur" }
            };

            var dtos = new List<LinkResponsableEnfantDto>
            {
                new LinkResponsableEnfantDto { EnfantId = 1, ResponsableId = 2, Affiliation = "Tuteur" }
            };

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetEnfantsByResponsableIdAsync(2))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkResponsableEnfantDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkResponsableEnfantService.GetEnfantsByResponsableIdAsync(2);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldHaveSingleItem();
            result.First().Affiliation.ShouldBe("Tuteur");
        }

        [Fact]
        public async Task GetEnfantsByResponsableIdAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.GetEnfantsByResponsableIdAsync(1));

            // Assert
            exception.Message.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetEnfantsByResponsableIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetEnfantsByResponsableIdAsync(1))
                .ReturnsAsync(new List<ResponsableEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkResponsableEnfantDto>>(It.IsAny<IEnumerable<ResponsableEnfant>>()))
                .Returns(new List<LinkResponsableEnfantDto>());

            // Act
            var result = await _linkResponsableEnfantService.GetEnfantsByResponsableIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ExistsLinkResponsableEnfantAsync_WhenLinkExists_ShouldReturnTrue()
        {
            // Arrange
            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(true);

            // Act
            var result = await _linkResponsableEnfantService.ExistsLinkResponsableEnfantAsync(1, 2);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkResponsableEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(false);

            // Act
            var result = await _linkResponsableEnfantService.ExistsLinkResponsableEnfantAsync(1, 2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenValid_ShouldReturnDto()
        {
            // Arrange
            var newLinkResponsableEnfantDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = 1,
                ResponsableId = 2,
                Affiliation = "Mère"
            };

            var newResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = newLinkResponsableEnfantDto.EnfantId,
                ResponsableId = newLinkResponsableEnfantDto.ResponsableId,
                Affiliation = newLinkResponsableEnfantDto.Affiliation
            };

            var createdResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = newLinkResponsableEnfantDto.EnfantId,
                ResponsableId = newLinkResponsableEnfantDto.ResponsableId,
                Affiliation = newLinkResponsableEnfantDto.Affiliation
            };

            var createdResponsableEnfantDto = new LinkResponsableEnfantDto
            {
                EnfantId = newLinkResponsableEnfantDto.EnfantId,
                ResponsableId = newLinkResponsableEnfantDto.ResponsableId,
                Affiliation = newLinkResponsableEnfantDto.Affiliation
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newLinkResponsableEnfantDto.ResponsableId, newLinkResponsableEnfantDto.EnfantId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<ResponsableEnfant>(newLinkResponsableEnfantDto))
                .Returns(newResponsableEnfant);

            _responsableEnfantRepositoryMock
                .Setup(r => r.AddAsync(newResponsableEnfant))
                .Returns(Task.CompletedTask);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(newLinkResponsableEnfantDto.ResponsableId, newLinkResponsableEnfantDto.EnfantId))
                .ReturnsAsync(createdResponsableEnfant);

            _mapperMock
                .Setup(m => m.Map<LinkResponsableEnfantDto>(createdResponsableEnfant))
                .Returns(createdResponsableEnfantDto);

            // Act
            var result = await _linkResponsableEnfantService.CreateLinkResponsableEnfantAsync(newLinkResponsableEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.ResponsableId.ShouldBe(newLinkResponsableEnfantDto.ResponsableId);
            result.EnfantId.ShouldBe(newLinkResponsableEnfantDto.EnfantId);
            result.Affiliation.ShouldBe("Mère");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkResponsableEnfantDto { EnfantId = 1, ResponsableId = 2 };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkResponsableEnfantAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkResponsableEnfantDto { EnfantId = 1, ResponsableId = 2 };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkResponsableEnfantAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenLinkAlreadyExists_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkResponsableEnfantDto { EnfantId = 1, ResponsableId = 1 };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newResponsableEnfantDto.ResponsableId, newResponsableEnfantDto.EnfantId))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkResponsableEnfantAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Ce lien existe déjà entre ce responsable et cet enfant.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = 1,
                ResponsableId = 1,
                Affiliation = "Père"
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newResponsableEnfantDto.ResponsableId, newResponsableEnfantDto.EnfantId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<ResponsableEnfant>(newResponsableEnfantDto))
                .Returns((ResponsableEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkResponsableEnfantAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du lien Responsable / Enfant : Le Mapping a échoué.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenGetLinkReturnsNull_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkResponsableEnfantDto
            {
                EnfantId = 1,
                ResponsableId = 2,
                Affiliation = "Tuteur"
            };

            var newResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = newResponsableEnfantDto.EnfantId,
                ResponsableId = newResponsableEnfantDto.ResponsableId,
                Affiliation = newResponsableEnfantDto.Affiliation
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newResponsableEnfantDto.ResponsableId, newResponsableEnfantDto.EnfantId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<ResponsableEnfant>(newResponsableEnfantDto))
                .Returns(newResponsableEnfant);

            _responsableEnfantRepositoryMock
                .Setup(r => r.AddAsync(newResponsableEnfant))
                .Returns(Task.CompletedTask);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(newResponsableEnfantDto.ResponsableId, newResponsableEnfantDto.EnfantId))
                .ReturnsAsync((ResponsableEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkResponsableEnfantAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Échec de la création du lien Responsable / Enfant.");
        }

        [Fact]
        public async Task UpdateLinkResponsableEnfantAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            var updateResponsableEnfantDto = new UpdateLinkResponsableEnfantDto 
            { 
                EnfantId = 1, 
                ResponsableId = 2, 
                Affiliation = "Tante" 
            };

            var existingResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = updateResponsableEnfantDto.EnfantId,
                ResponsableId = updateResponsableEnfantDto.ResponsableId,
                Affiliation = "Ancien"
            };

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(updateResponsableEnfantDto.ResponsableId, updateResponsableEnfantDto.EnfantId))
                .ReturnsAsync(existingResponsableEnfant);

            _mapperMock
                .Setup(m => m.Map(updateResponsableEnfantDto, existingResponsableEnfant))
                .Returns(existingResponsableEnfant); // peut être ignoré si Map void

            _responsableEnfantRepositoryMock
                .Setup(r => r.UpdateAsync(existingResponsableEnfant))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkResponsableEnfantService.UpdateLinkResponsableEnfantAsync(updateResponsableEnfantDto));
        }

        [Fact]
        public async Task UpdateLinkResponsableEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var updateResponsableEnfantDto = new UpdateLinkResponsableEnfantDto { EnfantId = 1, ResponsableId = 2, Affiliation = "Oncle" };

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(updateResponsableEnfantDto.ResponsableId, updateResponsableEnfantDto.EnfantId))
                .ReturnsAsync((ResponsableEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.UpdateLinkResponsableEnfantAsync(updateResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Aucun lien Responsable / Enfant trouvé à mettre à jour.");
        }

        [Fact]
        public async Task RemoveLinkResponsableEnfantAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.RemoveLinkAsync(2, 1))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkResponsableEnfantService.RemoveLinkResponsableEnfantAsync(1, 2));
        }

        [Fact]
        public async Task RemoveLinkResponsableEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.RemoveLinkResponsableEnfantAsync(1, 2));

            // Assert
            exception.Message.ShouldBe("Aucun lien Responsable / Enfant trouvé à supprimer.");
        }
    }
}