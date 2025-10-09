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
        private readonly ILinkGuardianChildService _linkResponsableEnfantService;
        private readonly Mock<IGuardianChildRepository> _responsableEnfantRepositoryMock;
        private readonly Mock<IChildRepository> _enfantRepositoryMock;
        private readonly Mock<IGuardianRepository> _responsableRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public LinkResponsableEnfantServiceTests()
        {
            _responsableEnfantRepositoryMock = new Mock<IGuardianChildRepository>();
            _enfantRepositoryMock = new Mock<IChildRepository>();
            _responsableRepositoryMock = new Mock<IGuardianRepository>();
            _mapperMock = new Mock<IMapper>();

            _linkResponsableEnfantService = new LinkGuardianChildService(
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

            var dtos = new List<LinkGuardianChildDto>
            {
                new() { ChildId = 1, GuardianId = 1, Relationship = "Père" }
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetGuardiansByChildIdAsync(1))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkResponsableEnfantService.GetGuardiansByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldHaveSingleItem();
            result.First().Relationship.ShouldBe("Père");
        }

        [Fact]
        public async Task GetResponsablesByEnfantIdAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.GetGuardiansByChildIdAsync(1));

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
                .Setup(r => r.GetGuardiansByChildIdAsync(1))
                .ReturnsAsync(new List<ResponsableEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(It.IsAny<IEnumerable<ResponsableEnfant>>()))
                .Returns(new List<LinkGuardianChildDto>());

            // Act
            var result = await _linkResponsableEnfantService.GetGuardiansByChildIdAsync(1);

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

            var dtos = new List<LinkGuardianChildDto>
            {
                new LinkGuardianChildDto { ChildId = 1, GuardianId = 2, Relationship = "Tuteur" }
            };

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetChildrenByGuardianIdAsync(2))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkResponsableEnfantService.GetChildrenByGuardianIdAsync(2);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldHaveSingleItem();
            result.First().Relationship.ShouldBe("Tuteur");
        }

        [Fact]
        public async Task GetEnfantsByResponsableIdAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.GetChildrenByGuardianIdAsync(1));

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
                .Setup(r => r.GetChildrenByGuardianIdAsync(1))
                .ReturnsAsync(new List<ResponsableEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(It.IsAny<IEnumerable<ResponsableEnfant>>()))
                .Returns(new List<LinkGuardianChildDto>());

            // Act
            var result = await _linkResponsableEnfantService.GetChildrenByGuardianIdAsync(1);

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
            var result = await _linkResponsableEnfantService.ExistsLinkGuardianChildAsync(1, 2);

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
            var result = await _linkResponsableEnfantService.ExistsLinkGuardianChildAsync(1, 2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenValid_ShouldReturnDto()
        {
            // Arrange
            var newLinkResponsableEnfantDto = new CreateLinkGuardianChildDto
            {
                ChildId = 1,
                GuardianId = 2,
                Relationship = "Mère"
            };

            var newResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = newLinkResponsableEnfantDto.ChildId,
                ResponsableId = newLinkResponsableEnfantDto.GuardianId,
                Affiliation = newLinkResponsableEnfantDto.Relationship
            };

            var createdResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = newLinkResponsableEnfantDto.ChildId,
                ResponsableId = newLinkResponsableEnfantDto.GuardianId,
                Affiliation = newLinkResponsableEnfantDto.Relationship
            };

            var createdResponsableEnfantDto = new LinkGuardianChildDto
            {
                ChildId = newLinkResponsableEnfantDto.ChildId,
                GuardianId = newLinkResponsableEnfantDto.GuardianId,
                Relationship = newLinkResponsableEnfantDto.Relationship
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newLinkResponsableEnfantDto.GuardianId, newLinkResponsableEnfantDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<ResponsableEnfant>(newLinkResponsableEnfantDto))
                .Returns(newResponsableEnfant);

            _responsableEnfantRepositoryMock
                .Setup(r => r.AddAsync(newResponsableEnfant))
                .Returns(Task.CompletedTask);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(newLinkResponsableEnfantDto.GuardianId, newLinkResponsableEnfantDto.ChildId))
                .ReturnsAsync(createdResponsableEnfant);

            _mapperMock
                .Setup(m => m.Map<LinkGuardianChildDto>(createdResponsableEnfant))
                .Returns(createdResponsableEnfantDto);

            // Act
            var result = await _linkResponsableEnfantService.CreateLinkGuardianChildAsync(newLinkResponsableEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.GuardianId.ShouldBe(newLinkResponsableEnfantDto.GuardianId);
            result.ChildId.ShouldBe(newLinkResponsableEnfantDto.ChildId);
            result.Relationship.ShouldBe("Mère");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkGuardianChildDto { ChildId = 1, GuardianId = 2 };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkGuardianChildAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenResponsableDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkGuardianChildDto { ChildId = 1, GuardianId = 2 };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkGuardianChildAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenLinkAlreadyExists_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkGuardianChildDto { ChildId = 1, GuardianId = 1 };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newResponsableEnfantDto.GuardianId, newResponsableEnfantDto.ChildId))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkGuardianChildAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Ce lien existe déjà entre ce responsable et cet enfant.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkGuardianChildDto
            {
                ChildId = 1,
                GuardianId = 1,
                Relationship = "Père"
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newResponsableEnfantDto.GuardianId, newResponsableEnfantDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<ResponsableEnfant>(newResponsableEnfantDto))
                .Returns((ResponsableEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkGuardianChildAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du lien Responsable / Enfant : Le Mapping a échoué.");
        }

        [Fact]
        public async Task CreateLinkResponsableEnfantAsync_WhenGetLinkReturnsNull_ShouldReturnFail()
        {
            // Arrange
            var newResponsableEnfantDto = new CreateLinkGuardianChildDto
            {
                ChildId = 1,
                GuardianId = 2,
                Relationship = "Tuteur"
            };

            var newResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = newResponsableEnfantDto.ChildId,
                ResponsableId = newResponsableEnfantDto.GuardianId,
                Affiliation = newResponsableEnfantDto.Relationship
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _responsableRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Responsable, bool>>>()))
                .ReturnsAsync(true);

            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(newResponsableEnfantDto.GuardianId, newResponsableEnfantDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<ResponsableEnfant>(newResponsableEnfantDto))
                .Returns(newResponsableEnfant);

            _responsableEnfantRepositoryMock
                .Setup(r => r.AddAsync(newResponsableEnfant))
                .Returns(Task.CompletedTask);

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(newResponsableEnfantDto.GuardianId, newResponsableEnfantDto.ChildId))
                .ReturnsAsync((ResponsableEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.CreateLinkGuardianChildAsync(newResponsableEnfantDto));

            // Assert
            exception.Message.ShouldBe("Échec de la création du lien Responsable / Enfant.");
        }

        [Fact]
        public async Task UpdateLinkResponsableEnfantAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            var updateResponsableEnfantDto = new UpdateLinkGuardianChildDto 
            { 
                ChildId = 1, 
                GuardianId = 2, 
                Relationship = "Tante" 
            };

            var existingResponsableEnfant = new ResponsableEnfant
            {
                EnfantId = updateResponsableEnfantDto.ChildId,
                ResponsableId = updateResponsableEnfantDto.GuardianId,
                Affiliation = "Ancien"
            };

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(updateResponsableEnfantDto.GuardianId, updateResponsableEnfantDto.ChildId))
                .ReturnsAsync(existingResponsableEnfant);

            _mapperMock
                .Setup(m => m.Map(updateResponsableEnfantDto, existingResponsableEnfant))
                .Returns(existingResponsableEnfant); // peut être ignoré si Map void

            _responsableEnfantRepositoryMock
                .Setup(r => r.UpdateAsync(existingResponsableEnfant))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkResponsableEnfantService.UpdateLinkGuardianChildAsync(updateResponsableEnfantDto));
        }

        [Fact]
        public async Task UpdateLinkResponsableEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var updateResponsableEnfantDto = new UpdateLinkGuardianChildDto { ChildId = 1, GuardianId = 2, Relationship = "Oncle" };

            _responsableEnfantRepositoryMock
                .Setup(r => r.GetLinkAsync(updateResponsableEnfantDto.GuardianId, updateResponsableEnfantDto.ChildId))
                .ReturnsAsync((ResponsableEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.UpdateLinkGuardianChildAsync(updateResponsableEnfantDto));

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
            await Should.NotThrowAsync(async () => await _linkResponsableEnfantService.RemoveLinkGuardianChildAsync(1, 2));
        }

        [Fact]
        public async Task RemoveLinkResponsableEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _responsableEnfantRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkResponsableEnfantService.RemoveLinkGuardianChildAsync(1, 2));

            // Assert
            exception.Message.ShouldBe("Aucun lien Responsable / Enfant trouvé à supprimer.");
        }
    }
}