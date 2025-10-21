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
    public class LinkAuthorizedPersonChildServiceTests
    {
        private readonly ILinkAuthorizedPersonChildService _linkAuthorizedPersonChildService;
        private readonly Mock<IAuthorizedPersonChildRepository> _authorizedPersonChildRepositoryMock;
        private readonly Mock<IAuthorizedPersonRepository> _authorizedPersonRepositoryMock;
        private readonly Mock<IChildRepository> _childRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public LinkAuthorizedPersonChildServiceTests()
        {
            _authorizedPersonChildRepositoryMock = new Mock<IAuthorizedPersonChildRepository>();
            _authorizedPersonRepositoryMock = new Mock<IAuthorizedPersonRepository>();
            _childRepositoryMock = new Mock<IChildRepository>();
            _mapperMock = new Mock<IMapper>();

            _linkAuthorizedPersonChildService = new LinkAuthorizedPersonChildService(
                _authorizedPersonChildRepositoryMock.Object,
                _childRepositoryMock.Object,
                _authorizedPersonRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetAuthorizedPeopleByChildIdAsync_WhenChildExists_ShouldReturnDtoList()
        {
            // Arrange
            var linkAuthorizedPersonChild = new List<AuthorizedPersonChild>
            {
                new() { ChildId = 1, AuthorizedPersonId = 1 },
                new() { ChildId = 1, AuthorizedPersonId = 2 }
            };

            var linkAuthorizedPersonChildDto = new List<LinkAuthorizedPersonChildDto>
            {
                new() { ChildId = 1, AuthorizedPersonId = 1 },
                new() { ChildId = 1, AuthorizedPersonId = 2 }
            };

            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.GetAuthorizedPeopleByChildIdAsync(1))
                .ReturnsAsync(linkAuthorizedPersonChild);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(linkAuthorizedPersonChild))
                .Returns(linkAuthorizedPersonChildDto);

            // Act
            var result = await _linkAuthorizedPersonChildService.GetAuthorizedPeopleByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetAuthorizedPeopleByChildIdAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.GetAuthorizedPeopleByChildIdAsync(1));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetAuthorizedPeopleByChildIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _childRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.GetAuthorizedPeopleByChildIdAsync(1))
                .ReturnsAsync(new List<AuthorizedPersonChild>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(It.IsAny<IEnumerable<AuthorizedPersonChild>>()))
                .Returns(new List<LinkAuthorizedPersonChildDto>());

            // Act
            var result = await _linkAuthorizedPersonChildService.GetAuthorizedPeopleByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildrenByAuthorizedPersonIdAsync_WhenAuthorizedPersonExists_ShouldReturnDtoList()
        {
            // Arrange
            var linkAuthorizedPersonChild = new List<AuthorizedPersonChild>
            {
                new() { ChildId = 1, AuthorizedPersonId = 2 },
                new() { ChildId = 2, AuthorizedPersonId = 2 }
            };

            var linkAuthorizedPersonChildDto = new List<LinkAuthorizedPersonChildDto>
            {
                new() { ChildId = 1, AuthorizedPersonId = 2 },
                new() { ChildId = 2, AuthorizedPersonId = 2 }
            };

            _authorizedPersonRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(r => r.GetChildrenByAuthorizedPersonIdAsync(2))
                .ReturnsAsync(linkAuthorizedPersonChild);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(linkAuthorizedPersonChild))
                .Returns(linkAuthorizedPersonChildDto);

            // Act
            var result = await _linkAuthorizedPersonChildService.GetChildrenByAuthorizedPersonIdAsync(2);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetChildrenByAuthorizedPersonIdAsync_WhenAuthorizedPersonDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _authorizedPersonRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.GetChildrenByAuthorizedPersonIdAsync(1));

            // Assert
            exception.Message.ShouldBe("La personne autorisée spécifiée n'existe pas.");
        }

        [Fact]
        public async Task GetChildrenByAuthorizedPersonIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _authorizedPersonRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.GetChildrenByAuthorizedPersonIdAsync(1))
                .ReturnsAsync(new List<AuthorizedPersonChild>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(It.IsAny<IEnumerable<AuthorizedPersonChild>>()))
                .Returns(new List<LinkAuthorizedPersonChildDto>());

            // Act
            var result = await _linkAuthorizedPersonChildService.GetChildrenByAuthorizedPersonIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ExistsLinkAuthorizedPersonChildAsync_WhenLinkExists_ShouldReturnTrue()
        {
            // Arrange
            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 1))
                .ReturnsAsync(true);

            // Act
            var result = await _linkAuthorizedPersonChildService.ExistsLinkAuthorizedPersonChildAsync(1, 1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkAuthorizedPersonChildAsync_WhenLinkDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 1))
                .ReturnsAsync(false);

            // Act
            var result = await _linkAuthorizedPersonChildService.ExistsLinkAuthorizedPersonChildAsync(1, 1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChildAsync_WhenValid_ShouldReturnDto()
        {
            // Arrange
            var newLinkAuthorizedPersonChildDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            var newLinkAuthorizedPersonChild = new AuthorizedPersonChild
            {
                ChildId = newLinkAuthorizedPersonChildDto.ChildId,
                AuthorizedPersonId = newLinkAuthorizedPersonChildDto.AuthorizedPersonId,
                Relationship = newLinkAuthorizedPersonChildDto.Relationship
            };

            var createdLinkAuthorizedPersonChild = new AuthorizedPersonChild
            {
                ChildId = newLinkAuthorizedPersonChildDto.ChildId,
                AuthorizedPersonId = newLinkAuthorizedPersonChildDto.AuthorizedPersonId,
                Relationship = newLinkAuthorizedPersonChildDto.Relationship
            };

            var createdLinkAuthorizedPersonChildDto = new LinkAuthorizedPersonChildDto
            {
                ChildId = newLinkAuthorizedPersonChildDto.ChildId,
                AuthorizedPersonId = newLinkAuthorizedPersonChildDto.AuthorizedPersonId,
                Relationship = newLinkAuthorizedPersonChildDto.Relationship
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(true);

            _childRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newLinkAuthorizedPersonChildDto.AuthorizedPersonId, newLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonChild>(newLinkAuthorizedPersonChildDto))
                .Returns(newLinkAuthorizedPersonChild);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.AddAsync(newLinkAuthorizedPersonChild))
                .Returns(Task.CompletedTask);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.GetLinkAsync(newLinkAuthorizedPersonChildDto.AuthorizedPersonId, newLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync(createdLinkAuthorizedPersonChild);

            _mapperMock
                .Setup(m => m.Map<LinkAuthorizedPersonChildDto>(createdLinkAuthorizedPersonChild))
                .Returns(createdLinkAuthorizedPersonChildDto);

            // Act
            var result = await _linkAuthorizedPersonChildService.CreateLinkAuthorizedPersonChildAsync(newLinkAuthorizedPersonChildDto);

            // Assert
            result.ShouldNotBeNull();
            result.AuthorizedPersonId.ShouldBe(newLinkAuthorizedPersonChildDto.AuthorizedPersonId);
            result.ChildId.ShouldBe(newLinkAuthorizedPersonChildDto.ChildId);
            result.Relationship.ShouldBe("Mère");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChildAsync_WhenAuthorizedPersonDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newLinkAuthorizedPersonChildDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(false);

            _childRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.CreateLinkAuthorizedPersonChildAsync(newLinkAuthorizedPersonChildDto));

            // Assert
            exception.Message.ShouldBe("La personne autorisée spécifiée n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChildAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newLinkAuthorizedPersonChildDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(true);

            _childRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.CreateLinkAuthorizedPersonChildAsync(newLinkAuthorizedPersonChildDto));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChildAsync_WhenLinkAlreadyExists_ShouldReturnFail()
        {
            // Arrange
            var newLinkAuthorizedPersonChildDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(true);

            _childRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newLinkAuthorizedPersonChildDto.AuthorizedPersonId, newLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.CreateLinkAuthorizedPersonChildAsync(newLinkAuthorizedPersonChildDto));

            // Assert
            exception.Message.ShouldBe("Ce lien existe déjà entre cette personne autorisée et cet enfant.");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChildAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newLinkAuthorizedPersonChildDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(true);

            _childRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newLinkAuthorizedPersonChildDto.AuthorizedPersonId, newLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonChild>(newLinkAuthorizedPersonChildDto))
                .Returns((AuthorizedPersonChild)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.CreateLinkAuthorizedPersonChildAsync(newLinkAuthorizedPersonChildDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du lien Personne Autorisée / Enfant : Le Mapping a échoué.");
        }

        [Fact]
        public async Task CreateLinkAuthorizedPersonChildAsync_WhenGetLinkReturnsNull_ShouldReturnFail()
        {
            // Arrange
            var newLinkAuthorizedPersonChildDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            var newLinkAuthorizedPersonChild = new AuthorizedPersonChild
            {
                ChildId = newLinkAuthorizedPersonChildDto.ChildId,
                AuthorizedPersonId = newLinkAuthorizedPersonChildDto.AuthorizedPersonId,
                Relationship = newLinkAuthorizedPersonChildDto.Relationship
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<AuthorizedPerson, bool>>>()))
                .ReturnsAsync(true);

            _childRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Child, bool>>>()))
                .ReturnsAsync(true);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newLinkAuthorizedPersonChildDto.AuthorizedPersonId, newLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonChild>(newLinkAuthorizedPersonChildDto))
                .Returns(newLinkAuthorizedPersonChild);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.AddAsync(newLinkAuthorizedPersonChild))
                .Returns(Task.CompletedTask);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.GetLinkAsync(newLinkAuthorizedPersonChildDto.AuthorizedPersonId, newLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync((AuthorizedPersonChild)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.CreateLinkAuthorizedPersonChildAsync(newLinkAuthorizedPersonChildDto));

            // Assert
            exception.Message.ShouldBe("Échec de la création du lien Personne Autorisée / Enfant.");
        }

        [Fact]
        public async Task UpdateLinkAuthorizedPersonChildAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            var updateLinkAuthorizedPersonChildDto = new UpdateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère",
                EmergencyContact = true,
                Comment = "Commentaire"
            };

            var existingLinkAuthorizedPersonChild = new AuthorizedPersonChild
            {
                ChildId = updateLinkAuthorizedPersonChildDto.ChildId,
                AuthorizedPersonId = updateLinkAuthorizedPersonChildDto.AuthorizedPersonId,
                Relationship = updateLinkAuthorizedPersonChildDto.Relationship,
                EmergencyContact = false,
                Comment = null
            };

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.GetLinkAsync(updateLinkAuthorizedPersonChildDto.AuthorizedPersonId, updateLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync(existingLinkAuthorizedPersonChild);

            _mapperMock
                .Setup(m => m.Map(updateLinkAuthorizedPersonChildDto, existingLinkAuthorizedPersonChild))
                .Returns(existingLinkAuthorizedPersonChild);

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.UpdateAsync(existingLinkAuthorizedPersonChild))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkAuthorizedPersonChildService.UpdateLinkAuthorizedPersonChildAsync(updateLinkAuthorizedPersonChildDto));
        }

        [Fact]
        public async Task UpdateLinkAuthorizedPersonChildAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var updateLinkAuthorizedPersonChildDto = new UpdateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère",
                EmergencyContact = true,
                Comment = "Commentaire"
            };

            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.GetLinkAsync(updateLinkAuthorizedPersonChildDto.AuthorizedPersonId, updateLinkAuthorizedPersonChildDto.ChildId))
                .ReturnsAsync((AuthorizedPersonChild)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.UpdateLinkAuthorizedPersonChildAsync(updateLinkAuthorizedPersonChildDto));

            // Assert
            exception.Message.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas.");
        }

        [Fact]
        public async Task RemoveLinkAuthorizedPersonChildAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 2))
                .ReturnsAsync(true);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkAuthorizedPersonChildService.RemoveLinkAuthorizedPersonChildAsync(2, 1));
        }

        [Fact]
        public async Task RemoveLinkAuthorizedPersonChildAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _authorizedPersonChildRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 2))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkAuthorizedPersonChildService.RemoveLinkAuthorizedPersonChildAsync(2, 1));

            // Assert
            exception.Message.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas.");
        }
    }
}