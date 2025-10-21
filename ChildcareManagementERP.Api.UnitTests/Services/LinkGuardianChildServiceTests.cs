using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;
using ChildcareManagementERP.Api.Services;
using Moq;
using Shouldly;
using System.Linq.Expressions;

namespace ChildcareManagementERP.Api.UnitTests.Services
{
    public class LinkGuardianChildServiceTests
    {
        private readonly ILinkGuardianChildService _linkGuardianChildService;
        private readonly Mock<IGuardianChildRepository> _guardianChildRepositoryMock;
        private readonly Mock<IChildRepository> _childRepositoryMock;
        private readonly Mock<IGuardianRepository> _guardianRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public LinkGuardianChildServiceTests()
        {
            _guardianChildRepositoryMock = new Mock<IGuardianChildRepository>();
            _childRepositoryMock = new Mock<IChildRepository>();
            _guardianRepositoryMock = new Mock<IGuardianRepository>();
            _mapperMock = new Mock<IMapper>();

            _linkGuardianChildService = new LinkGuardianChildService(
                _guardianChildRepositoryMock.Object,
                _childRepositoryMock.Object,
                _guardianRepositoryMock.Object,
                _mapperMock.Object
            );
        }

        [Fact]
        public async Task GetGuardiansByChildIdAsync_WhenChildExist_ShouldReturnDtoList()
        {
            // Arrange
            var linkGuardianChild = new List<GuardianChild>
            {
                new() { ChildId = 1, GuardianId = 1, Relationship = "Père" }
            };

            var linkGuardianChildDto = new List<LinkGuardianChildDto>
            {
                new() { ChildId = 1, GuardianId = 1, Relationship = "Père" }
            };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.GetGuardiansByChildIdAsync(1))
                .ReturnsAsync(linkGuardianChild);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(linkGuardianChild))
                .Returns(linkGuardianChildDto);

            // Act
            var result = await _linkGuardianChildService.GetGuardiansByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldHaveSingleItem();
            result.First().Relationship.ShouldBe("Père");
        }

        [Fact]
        public async Task GetGuardiansByChildIdAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.GetGuardiansByChildIdAsync(1));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetGuardiansByChildIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.GetGuardiansByChildIdAsync(1))
                .ReturnsAsync(new List<GuardianChild>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(It.IsAny<IEnumerable<GuardianChild>>()))
                .Returns(new List<LinkGuardianChildDto>());

            // Act
            var result = await _linkGuardianChildService.GetGuardiansByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildrenByGuardianIdAsync_WhenGuardianExist_ShouldReturnDtoList()
        {
            // Arrange
            var linkGuardianChild = new List<GuardianChild>
            {
                new GuardianChild { ChildId = 1, GuardianId = 2, Relationship = "Tuteur" }
            };

            var linkGuardianChildDto = new List<LinkGuardianChildDto>
            {
                new LinkGuardianChildDto { ChildId = 1, GuardianId = 2, Relationship = "Tuteur" }
            };

            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.GetChildrenByGuardianIdAsync(2))
                .ReturnsAsync(linkGuardianChild);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(linkGuardianChild))
                .Returns(linkGuardianChildDto);

            // Act
            var result = await _linkGuardianChildService.GetChildrenByGuardianIdAsync(2);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldHaveSingleItem();
            result.First().Relationship.ShouldBe("Tuteur");
        }

        [Fact]
        public async Task GetChildrenByGuardianIdAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.GetChildrenByGuardianIdAsync(1));

            // Assert
            exception.Message.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetChildrenByGuardianIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.GetChildrenByGuardianIdAsync(1))
                .ReturnsAsync(new List<GuardianChild>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkGuardianChildDto>>(It.IsAny<IEnumerable<GuardianChild>>()))
                .Returns(new List<LinkGuardianChildDto>());

            // Act
            var result = await _linkGuardianChildService.GetChildrenByGuardianIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ExistsLinkGuardianChildAsync_WhenLinkExists_ShouldReturnTrue()
        {
            // Arrange
            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(true);

            // Act
            var result = await _linkGuardianChildService.ExistsLinkGuardianChildAsync(1, 2);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkGuardianChildAsync_WhenLinkDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(false);

            // Act
            var result = await _linkGuardianChildService.ExistsLinkGuardianChildAsync(1, 2);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkGuardianChildAsync_WhenValid_ShouldReturnDto()
        {
            // Arrange
            var newLinkGuardianChildDto = new CreateLinkGuardianChildDto
            {
                ChildId = 1,
                GuardianId = 2,
                Relationship = "Mère"
            };

            var newLinkGuardianChild = new GuardianChild
            {
                ChildId = newLinkGuardianChildDto.ChildId,
                GuardianId = newLinkGuardianChildDto.GuardianId,
                Relationship = newLinkGuardianChildDto.Relationship
            };

            var createdLinkGuardianChild = new GuardianChild
            {
                ChildId = newLinkGuardianChildDto.ChildId,
                GuardianId = newLinkGuardianChildDto.GuardianId,
                Relationship = newLinkGuardianChildDto.Relationship
            };

            var createdLinkGuardianChildDto = new LinkGuardianChildDto
            {
                ChildId = newLinkGuardianChildDto.ChildId,
                GuardianId = newLinkGuardianChildDto.GuardianId,
                Relationship = newLinkGuardianChildDto.Relationship
            };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(newLinkGuardianChildDto.GuardianId, newLinkGuardianChildDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<GuardianChild>(newLinkGuardianChildDto))
                .Returns(newLinkGuardianChild);

            _guardianChildRepositoryMock
                .Setup(r => r.AddAsync(newLinkGuardianChild))
                .Returns(Task.CompletedTask);

            _guardianChildRepositoryMock
                .Setup(r => r.GetLinkAsync(newLinkGuardianChildDto.GuardianId, newLinkGuardianChildDto.ChildId))
                .ReturnsAsync(createdLinkGuardianChild);

            _mapperMock
                .Setup(m => m.Map<LinkGuardianChildDto>(createdLinkGuardianChild))
                .Returns(createdLinkGuardianChildDto);

            // Act
            var result = await _linkGuardianChildService.CreateLinkGuardianChildAsync(newLinkGuardianChildDto);

            // Assert
            result.ShouldNotBeNull();
            result.GuardianId.ShouldBe(newLinkGuardianChildDto.GuardianId);
            result.ChildId.ShouldBe(newLinkGuardianChildDto.ChildId);
            result.Relationship.ShouldBe("Mère");
        }

        [Fact]
        public async Task CreateLinkGuardianChildAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newLinkGuardianChildDto = new CreateLinkGuardianChildDto { ChildId = 1, GuardianId = 2 };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(false);

            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.CreateLinkGuardianChildAsync(newLinkGuardianChildDto));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkGuardianChildAsync_WhenGuardianDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newLinkGuardianChildDto = new CreateLinkGuardianChildDto { ChildId = 1, GuardianId = 2 };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.CreateLinkGuardianChildAsync(newLinkGuardianChildDto));

            // Assert
            exception.Message.ShouldBe("Le responsable spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkGuardianChildAsync_WhenLinkAlreadyExists_ShouldReturnFail()
        {
            // Arrange
            var newLinkGuardianChildDto = new CreateLinkGuardianChildDto { ChildId = 1, GuardianId = 1 };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(newLinkGuardianChildDto.GuardianId, newLinkGuardianChildDto.ChildId))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.CreateLinkGuardianChildAsync(newLinkGuardianChildDto));

            // Assert
            exception.Message.ShouldBe("Ce lien existe déjà entre ce responsable et cet enfant.");
        }

        [Fact]
        public async Task CreateLinkGuardianChildAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newLinkGuardianChildDto = new CreateLinkGuardianChildDto
            {
                ChildId = 1,
                GuardianId = 1,
                Relationship = "Père"
            };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(newLinkGuardianChildDto.GuardianId, newLinkGuardianChildDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<GuardianChild>(newLinkGuardianChildDto))
                .Returns((GuardianChild)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.CreateLinkGuardianChildAsync(newLinkGuardianChildDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du lien Responsable / Enfant : Le Mapping a échoué.");
        }

        [Fact]
        public async Task CreateLinkGuardianChildAsync_WhenGetLinkReturnsNull_ShouldReturnFail()
        {
            // Arrange
            var newLinkGuardianChildDto = new CreateLinkGuardianChildDto
            {
                ChildId = 1,
                GuardianId = 2,
                Relationship = "Tuteur"
            };

            var newLinkGuardianChild = new GuardianChild
            {
                ChildId = newLinkGuardianChildDto.ChildId,
                GuardianId = newLinkGuardianChildDto.GuardianId,
                Relationship = newLinkGuardianChildDto.Relationship
            };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _guardianRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Guardian, bool>>>()))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(newLinkGuardianChildDto.GuardianId, newLinkGuardianChildDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<GuardianChild>(newLinkGuardianChildDto))
                .Returns(newLinkGuardianChild);

            _guardianChildRepositoryMock
                .Setup(r => r.AddAsync(newLinkGuardianChild))
                .Returns(Task.CompletedTask);

            _guardianChildRepositoryMock
                .Setup(r => r.GetLinkAsync(newLinkGuardianChildDto.GuardianId, newLinkGuardianChildDto.ChildId))
                .ReturnsAsync((GuardianChild)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.CreateLinkGuardianChildAsync(newLinkGuardianChildDto));

            // Assert
            exception.Message.ShouldBe("Échec de la création du lien Responsable / Enfant.");
        }

        [Fact]
        public async Task UpdateLinkGuardianChildAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            var updateLinkGuardianChildDto = new UpdateLinkGuardianChildDto 
            { 
                ChildId = 1, 
                GuardianId = 2, 
                Relationship = "Tante" 
            };

            var existingLinkGuardianChild = new GuardianChild
            {
                ChildId = updateLinkGuardianChildDto.ChildId,
                GuardianId = updateLinkGuardianChildDto.GuardianId,
                Relationship = "Ancien"
            };

            _guardianChildRepositoryMock
                .Setup(r => r.GetLinkAsync(updateLinkGuardianChildDto.GuardianId, updateLinkGuardianChildDto.ChildId))
                .ReturnsAsync(existingLinkGuardianChild);

            _mapperMock
                .Setup(m => m.Map(updateLinkGuardianChildDto, existingLinkGuardianChild))
                .Returns(existingLinkGuardianChild); // peut être ignoré si Map void

            _guardianChildRepositoryMock
                .Setup(r => r.UpdateAsync(existingLinkGuardianChild))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkGuardianChildService.UpdateLinkGuardianChildAsync(updateLinkGuardianChildDto));
        }

        [Fact]
        public async Task UpdateLinkGuardianChildAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var updateLinkGuardianChildDto = new UpdateLinkGuardianChildDto { ChildId = 1, GuardianId = 2, Relationship = "Oncle" };

            _guardianChildRepositoryMock
                .Setup(r => r.GetLinkAsync(updateLinkGuardianChildDto.GuardianId, updateLinkGuardianChildDto.ChildId))
                .ReturnsAsync((GuardianChild)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.UpdateLinkGuardianChildAsync(updateLinkGuardianChildDto));

            // Assert
            exception.Message.ShouldBe("Aucun lien Responsable / Enfant trouvé à mettre à jour.");
        }

        [Fact]
        public async Task RemoveLinkGuardianChildAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(true);

            _guardianChildRepositoryMock
                .Setup(r => r.RemoveLinkAsync(2, 1))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkGuardianChildService.RemoveLinkGuardianChildAsync(1, 2));
        }

        [Fact]
        public async Task RemoveLinkGuardianChildAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _guardianChildRepositoryMock
                .Setup(r => r.LinkExistsAsync(2, 1))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkGuardianChildService.RemoveLinkGuardianChildAsync(1, 2));

            // Assert
            exception.Message.ShouldBe("Aucun lien Responsable / Enfant trouvé à supprimer.");
        }
    }
}