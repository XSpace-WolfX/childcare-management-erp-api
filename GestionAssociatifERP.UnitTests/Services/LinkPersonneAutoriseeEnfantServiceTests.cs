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
    public class LinkPersonneAutoriseeEnfantServiceTests
    {
        private readonly ILinkAuthorizedPersonChildService _linkPersonneAutoriseeEnfantService;
        private readonly Mock<IAuthorizedPersonChildRepository> _personneAutoriseeEnfantRepositoryMock;
        private readonly Mock<IAuthorizedPersonRepository> _personneAutoriseeRepositoryMock;
        private readonly Mock<IChildRepository> _enfantRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public LinkPersonneAutoriseeEnfantServiceTests()
        {
            _personneAutoriseeEnfantRepositoryMock = new Mock<IAuthorizedPersonChildRepository>();
            _personneAutoriseeRepositoryMock = new Mock<IAuthorizedPersonRepository>();
            _enfantRepositoryMock = new Mock<IChildRepository>();
            _mapperMock = new Mock<IMapper>();

            _linkPersonneAutoriseeEnfantService = new LinkAuthorizedPersonChildService(
                _personneAutoriseeEnfantRepositoryMock.Object,
                _enfantRepositoryMock.Object,
                _personneAutoriseeRepositoryMock.Object,
                _mapperMock.Object);
        }

        [Fact]
        public async Task GetPersonnesAutoriseesByEnfantIdAsync_WhenEnfantExists_ShouldReturnDtoList()
        {
            // Arrange
            var links = new List<PersonneAutoriseeEnfant>
            {
                new() { EnfantId = 1, PersonneAutoriseeId = 1 },
                new() { EnfantId = 1, PersonneAutoriseeId = 2 }
            };

            var dtos = new List<LinkAuthorizedPersonChildDto>
            {
                new() { ChildId = 1, AuthorizedPersonId = 1 },
                new() { ChildId = 1, AuthorizedPersonId = 2 }
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetAuthorizedPeopleByChildIdAsync(1))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetAuthorizedPeopleByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetPersonnesAutoriseesByEnfantIdAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetAuthorizedPeopleByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("L'enfant spécifié n'existe pas");
        }

        [Fact]
        public async Task GetPersonnesAutoriseesByEnfantIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetAuthorizedPeopleByChildIdAsync(1))
                .ReturnsAsync(new List<PersonneAutoriseeEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(It.IsAny<IEnumerable<PersonneAutoriseeEnfant>>()))
                .Returns(new List<LinkAuthorizedPersonChildDto>());

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetAuthorizedPeopleByChildIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetEnfantsByPersonneAutoriseeIdAsync_WhenPersonneAutoriseeExists_ShouldReturnDtoList()
        {
            // Arrange
            var links = new List<PersonneAutoriseeEnfant>
            {
                new() { EnfantId = 1, PersonneAutoriseeId = 2 },
                new() { EnfantId = 2, PersonneAutoriseeId = 2 }
            };

            var dtos = new List<LinkAuthorizedPersonChildDto>
            {
                new() { ChildId = 1, AuthorizedPersonId = 2 },
                new() { ChildId = 2, AuthorizedPersonId = 2 }
            };

            _personneAutoriseeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(r => r.GetChildrenByAuthorizedPersonIdAsync(2))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetChildrenByAuthorizedPersonIdAsync(2);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetEnfantsByPersonneAutoriseeIdAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetChildrenByAuthorizedPersonIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("La personne autorisée spécifiée n'existe pas");
        }

        [Fact]
        public async Task GetEnfantsByPersonneAutoriseeIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetChildrenByAuthorizedPersonIdAsync(1))
                .ReturnsAsync(new List<PersonneAutoriseeEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkAuthorizedPersonChildDto>>(It.IsAny<IEnumerable<PersonneAutoriseeEnfant>>()))
                .Returns(new List<LinkAuthorizedPersonChildDto>());

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetChildrenByAuthorizedPersonIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeEmpty();
        }

        [Fact]
        public async Task ExistsLinkPersonneAutoriseeEnfantAsync_WhenLinkExists_ShouldReturnTrue()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 1))
                .ReturnsAsync(true);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.ExistsLinkAuthorizedPersonChildAsync(1, 1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkPersonneAutoriseeEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 1))
                .ReturnsAsync(false);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.ExistsLinkAuthorizedPersonChildAsync(1, 1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenValid_ShouldReturnDto()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            var newPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = newPersonneAutoriseeEnfantDto.ChildId,
                PersonneAutoriseeId = newPersonneAutoriseeEnfantDto.AuthorizedPersonId,
                Affiliation = newPersonneAutoriseeEnfantDto.Relationship
            };

            var createdPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = newPersonneAutoriseeEnfantDto.ChildId,
                PersonneAutoriseeId = newPersonneAutoriseeEnfantDto.AuthorizedPersonId,
                Affiliation = newPersonneAutoriseeEnfantDto.Relationship
            };

            var createdPersonneAutoriseeEnfantDto = new LinkAuthorizedPersonChildDto
            {
                ChildId = newPersonneAutoriseeEnfantDto.ChildId,
                AuthorizedPersonId = newPersonneAutoriseeEnfantDto.AuthorizedPersonId,
                Relationship = newPersonneAutoriseeEnfantDto.Relationship
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.AuthorizedPersonId, newPersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeEnfant>(newPersonneAutoriseeEnfantDto))
                .Returns(newPersonneAutoriseeEnfant);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.AddAsync(newPersonneAutoriseeEnfant))
                .Returns(Task.CompletedTask);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(newPersonneAutoriseeEnfantDto.AuthorizedPersonId, newPersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync(createdPersonneAutoriseeEnfant);

            _mapperMock
                .Setup(m => m.Map<LinkAuthorizedPersonChildDto>(createdPersonneAutoriseeEnfant))
                .Returns(createdPersonneAutoriseeEnfantDto);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.CreateLinkAuthorizedPersonChildAsync(newPersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
            result.Data.ShouldNotBeNull();
            result.Data.PersonneAutoriseeId.ShouldBe(newPersonneAutoriseeEnfantDto.AuthorizedPersonId);
            result.Data.EnfantId.ShouldBe(newPersonneAutoriseeEnfantDto.ChildId);
            result.Data.Affiliation.ShouldBe("Mère");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(false);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.CreateLinkAuthorizedPersonChildAsync(newPersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("La personne autorisée spécifiée n'existe pas");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.CreateLinkAuthorizedPersonChildAsync(newPersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("L'enfant spécifié n'existe pas");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenLinkAlreadyExists_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.AuthorizedPersonId, newPersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync(true);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.CreateLinkAuthorizedPersonChildAsync(newPersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("Ce lien existe déjà entre cette personne autorisée et cet enfant");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.AuthorizedPersonId, newPersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeEnfant>(newPersonneAutoriseeEnfantDto))
                .Returns((PersonneAutoriseeEnfant)null!);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.CreateLinkAuthorizedPersonChildAsync(newPersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("Erreur lors de la création du lien Personne Autorisée / Enfant : Le Mapping a échoué");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenGetLinkReturnsNull_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère"
            };

            var newPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = newPersonneAutoriseeEnfantDto.ChildId,
                PersonneAutoriseeId = newPersonneAutoriseeEnfantDto.AuthorizedPersonId,
                Affiliation = newPersonneAutoriseeEnfantDto.Relationship
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.AuthorizedPersonId, newPersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeEnfant>(newPersonneAutoriseeEnfantDto))
                .Returns(newPersonneAutoriseeEnfant);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.AddAsync(newPersonneAutoriseeEnfant))
                .Returns(Task.CompletedTask);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(newPersonneAutoriseeEnfantDto.AuthorizedPersonId, newPersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync((PersonneAutoriseeEnfant)null!);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.CreateLinkAuthorizedPersonChildAsync(newPersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("Échec de la création du lien Personne Autorisée / Enfant");
            result.Data.ShouldBeNull();
        }

        [Fact]
        public async Task UpdateLinkPersonneAutoriseeEnfantAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            var updatePersonneAutoriseeEnfantDto = new UpdateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère",
                EmergencyContact = true,
                Comment = "Commentaire"
            };

            var existingPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = updatePersonneAutoriseeEnfantDto.ChildId,
                PersonneAutoriseeId = updatePersonneAutoriseeEnfantDto.AuthorizedPersonId,
                Affiliation = updatePersonneAutoriseeEnfantDto.Relationship,
                ContactUrgence = false,
                Commentaire = null
            };

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(updatePersonneAutoriseeEnfantDto.AuthorizedPersonId, updatePersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync(existingPersonneAutoriseeEnfant);

            _mapperMock
                .Setup(m => m.Map(updatePersonneAutoriseeEnfantDto, existingPersonneAutoriseeEnfant))
                .Returns(existingPersonneAutoriseeEnfant);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.UpdateAsync(existingPersonneAutoriseeEnfant))
                .Returns(Task.CompletedTask);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.UpdateLinkAuthorizedPersonChildAsync(updatePersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
        }

        [Fact]
        public async Task UpdateLinkPersonneAutoriseeEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var updatePersonneAutoriseeEnfantDto = new UpdateLinkAuthorizedPersonChildDto
            {
                ChildId = 1,
                AuthorizedPersonId = 2,
                Relationship = "Mère",
                EmergencyContact = true,
                Comment = "Commentaire"
            };

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(updatePersonneAutoriseeEnfantDto.AuthorizedPersonId, updatePersonneAutoriseeEnfantDto.ChildId))
                .ReturnsAsync((PersonneAutoriseeEnfant)null!);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.UpdateLinkAuthorizedPersonChildAsync(updatePersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas");
        }

        [Fact]
        public async Task RemoveLinkPersonneAutoriseeEnfantAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 2))
                .ReturnsAsync(true);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.RemoveLinkAuthorizedPersonChildAsync(2, 1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeTrue();
        }

        [Fact]
        public async Task RemoveLinkPersonneAutoriseeEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 2))
                .ReturnsAsync(false);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.RemoveLinkAuthorizedPersonChildAsync(2, 1);

            // Assert
            result.ShouldNotBeNull();
            result.Success.ShouldBeFalse();
            result.Message.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas");
        }
    }
}