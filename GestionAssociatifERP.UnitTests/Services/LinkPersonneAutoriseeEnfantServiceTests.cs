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
        private readonly ILinkPersonneAutoriseeEnfantService _linkPersonneAutoriseeEnfantService;
        private readonly Mock<IPersonneAutoriseeEnfantRepository> _personneAutoriseeEnfantRepositoryMock;
        private readonly Mock<IPersonneAutoriseeRepository> _personneAutoriseeRepositoryMock;
        private readonly Mock<IEnfantRepository> _enfantRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public LinkPersonneAutoriseeEnfantServiceTests()
        {
            _personneAutoriseeEnfantRepositoryMock = new Mock<IPersonneAutoriseeEnfantRepository>();
            _personneAutoriseeRepositoryMock = new Mock<IPersonneAutoriseeRepository>();
            _enfantRepositoryMock = new Mock<IEnfantRepository>();
            _mapperMock = new Mock<IMapper>();

            _linkPersonneAutoriseeEnfantService = new LinkPersonneAutoriseeEnfantService(
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

            var dtos = new List<LinkPersonneAutoriseeEnfantDto>
            {
                new() { EnfantId = 1, PersonneAutoriseeId = 1 },
                new() { EnfantId = 1, PersonneAutoriseeId = 2 }
            };

            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetPersonnesAutoriseesByEnfantIdAsync(1))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkPersonneAutoriseeEnfantDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetPersonnesAutoriseesByEnfantIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetPersonnesAutoriseesByEnfantIdAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.GetPersonnesAutoriseesByEnfantIdAsync(1));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task GetPersonnesAutoriseesByEnfantIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _enfantRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetPersonnesAutoriseesByEnfantIdAsync(1))
                .ReturnsAsync(new List<PersonneAutoriseeEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkPersonneAutoriseeEnfantDto>>(It.IsAny<IEnumerable<PersonneAutoriseeEnfant>>()))
                .Returns(new List<LinkPersonneAutoriseeEnfantDto>());

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetPersonnesAutoriseesByEnfantIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
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

            var dtos = new List<LinkPersonneAutoriseeEnfantDto>
            {
                new() { EnfantId = 1, PersonneAutoriseeId = 2 },
                new() { EnfantId = 2, PersonneAutoriseeId = 2 }
            };

            _personneAutoriseeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(r => r.GetEnfantsByPersonneAutoriseeIdAsync(2))
                .ReturnsAsync(links);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkPersonneAutoriseeEnfantDto>>(links))
                .Returns(dtos);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetEnfantsByPersonneAutoriseeIdAsync(2);

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
        }

        [Fact]
        public async Task GetEnfantsByPersonneAutoriseeIdAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.GetEnfantsByPersonneAutoriseeIdAsync(1));

            // Assert
            exception.Message.ShouldBe("La personne autorisée spécifiée n'existe pas.");
        }

        [Fact]
        public async Task GetEnfantsByPersonneAutoriseeIdAsync_WhenNoLinks_ShouldReturnEmptyList()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(r => r.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetEnfantsByPersonneAutoriseeIdAsync(1))
                .ReturnsAsync(new List<PersonneAutoriseeEnfant>());

            _mapperMock
                .Setup(m => m.Map<IEnumerable<LinkPersonneAutoriseeEnfantDto>>(It.IsAny<IEnumerable<PersonneAutoriseeEnfant>>()))
                .Returns(new List<LinkPersonneAutoriseeEnfantDto>());

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.GetEnfantsByPersonneAutoriseeIdAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task ExistsLinkPersonneAutoriseeEnfantAsync_WhenLinkExists_ShouldReturnTrue()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 1))
                .ReturnsAsync(true);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.ExistsLinkPersonneAutoriseeEnfantAsync(1, 1);

            // Assert
            result.ShouldBeTrue();
        }

        [Fact]
        public async Task ExistsLinkPersonneAutoriseeEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFalse()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 1))
                .ReturnsAsync(false);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.ExistsLinkPersonneAutoriseeEnfantAsync(1, 1);

            // Assert
            result.ShouldBeFalse();
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenValid_ShouldReturnDto()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère"
            };

            var newPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = newPersonneAutoriseeEnfantDto.EnfantId,
                PersonneAutoriseeId = newPersonneAutoriseeEnfantDto.PersonneAutoriseeId,
                Affiliation = newPersonneAutoriseeEnfantDto.Affiliation
            };

            var createdPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = newPersonneAutoriseeEnfantDto.EnfantId,
                PersonneAutoriseeId = newPersonneAutoriseeEnfantDto.PersonneAutoriseeId,
                Affiliation = newPersonneAutoriseeEnfantDto.Affiliation
            };

            var createdPersonneAutoriseeEnfantDto = new LinkPersonneAutoriseeEnfantDto
            {
                EnfantId = newPersonneAutoriseeEnfantDto.EnfantId,
                PersonneAutoriseeId = newPersonneAutoriseeEnfantDto.PersonneAutoriseeId,
                Affiliation = newPersonneAutoriseeEnfantDto.Affiliation
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.PersonneAutoriseeId, newPersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeEnfant>(newPersonneAutoriseeEnfantDto))
                .Returns(newPersonneAutoriseeEnfant);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.AddAsync(newPersonneAutoriseeEnfant))
                .Returns(Task.CompletedTask);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(newPersonneAutoriseeEnfantDto.PersonneAutoriseeId, newPersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync(createdPersonneAutoriseeEnfant);

            _mapperMock
                .Setup(m => m.Map<LinkPersonneAutoriseeEnfantDto>(createdPersonneAutoriseeEnfant))
                .Returns(createdPersonneAutoriseeEnfantDto);

            // Act
            var result = await _linkPersonneAutoriseeEnfantService.CreateLinkPersonneAutoriseeEnfantAsync(newPersonneAutoriseeEnfantDto);

            // Assert
            result.ShouldNotBeNull();
            result.PersonneAutoriseeId.ShouldBe(newPersonneAutoriseeEnfantDto.PersonneAutoriseeId);
            result.EnfantId.ShouldBe(newPersonneAutoriseeEnfantDto.EnfantId);
            result.Affiliation.ShouldBe("Mère");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(false);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.CreateLinkPersonneAutoriseeEnfantAsync(newPersonneAutoriseeEnfantDto));

            // Assert
            exception.Message.ShouldBe("La personne autorisée spécifiée n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenEnfantDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.CreateLinkPersonneAutoriseeEnfantAsync(newPersonneAutoriseeEnfantDto));

            // Assert
            exception.Message.ShouldBe("L'enfant spécifié n'existe pas.");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenLinkAlreadyExists_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.PersonneAutoriseeId, newPersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync(true);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.CreateLinkPersonneAutoriseeEnfantAsync(newPersonneAutoriseeEnfantDto));

            // Assert
            exception.Message.ShouldBe("Ce lien existe déjà entre cette personne autorisée et cet enfant.");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère"
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.PersonneAutoriseeId, newPersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeEnfant>(newPersonneAutoriseeEnfantDto))
                .Returns((PersonneAutoriseeEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.CreateLinkPersonneAutoriseeEnfantAsync(newPersonneAutoriseeEnfantDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création du lien Personne Autorisée / Enfant : Le Mapping a échoué.");
        }

        [Fact]
        public async Task CreateLinkPersonneAutoriseeEnfantAsync_WhenGetLinkReturnsNull_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeEnfantDto = new CreateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère"
            };

            var newPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = newPersonneAutoriseeEnfantDto.EnfantId,
                PersonneAutoriseeId = newPersonneAutoriseeEnfantDto.PersonneAutoriseeId,
                Affiliation = newPersonneAutoriseeEnfantDto.Affiliation
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<PersonneAutorisee, bool>>>()))
                .ReturnsAsync(true);

            _enfantRepositoryMock
                .Setup(repo => repo.ExistsAsync(It.IsAny<Expression<Func<Enfant, bool>>>()))
                .ReturnsAsync(true);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(newPersonneAutoriseeEnfantDto.PersonneAutoriseeId, newPersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync(false);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeEnfant>(newPersonneAutoriseeEnfantDto))
                .Returns(newPersonneAutoriseeEnfant);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.AddAsync(newPersonneAutoriseeEnfant))
                .Returns(Task.CompletedTask);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(newPersonneAutoriseeEnfantDto.PersonneAutoriseeId, newPersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync((PersonneAutoriseeEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.CreateLinkPersonneAutoriseeEnfantAsync(newPersonneAutoriseeEnfantDto));

            // Assert
            exception.Message.ShouldBe("Échec de la création du lien Personne Autorisée / Enfant.");
        }

        [Fact]
        public async Task UpdateLinkPersonneAutoriseeEnfantAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            var updatePersonneAutoriseeEnfantDto = new UpdateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère",
                ContactUrgence = true,
                Commentaire = "Commentaire"
            };

            var existingPersonneAutoriseeEnfant = new PersonneAutoriseeEnfant
            {
                EnfantId = updatePersonneAutoriseeEnfantDto.EnfantId,
                PersonneAutoriseeId = updatePersonneAutoriseeEnfantDto.PersonneAutoriseeId,
                Affiliation = updatePersonneAutoriseeEnfantDto.Affiliation,
                ContactUrgence = false,
                Commentaire = null
            };

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(updatePersonneAutoriseeEnfantDto.PersonneAutoriseeId, updatePersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync(existingPersonneAutoriseeEnfant);

            _mapperMock
                .Setup(m => m.Map(updatePersonneAutoriseeEnfantDto, existingPersonneAutoriseeEnfant))
                .Returns(existingPersonneAutoriseeEnfant);

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.UpdateAsync(existingPersonneAutoriseeEnfant))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkPersonneAutoriseeEnfantService.UpdateLinkPersonneAutoriseeEnfantAsync(updatePersonneAutoriseeEnfantDto));
        }

        [Fact]
        public async Task UpdateLinkPersonneAutoriseeEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var updatePersonneAutoriseeEnfantDto = new UpdateLinkPersonneAutoriseeEnfantDto
            {
                EnfantId = 1,
                PersonneAutoriseeId = 2,
                Affiliation = "Mère",
                ContactUrgence = true,
                Commentaire = "Commentaire"
            };

            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.GetLinkAsync(updatePersonneAutoriseeEnfantDto.PersonneAutoriseeId, updatePersonneAutoriseeEnfantDto.EnfantId))
                .ReturnsAsync((PersonneAutoriseeEnfant)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.UpdateLinkPersonneAutoriseeEnfantAsync(updatePersonneAutoriseeEnfantDto));

            // Assert
            exception.Message.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas.");
        }

        [Fact]
        public async Task RemoveLinkPersonneAutoriseeEnfantAsync_WhenLinkExists_ShouldReturnSuccess()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 2))
                .ReturnsAsync(true);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _linkPersonneAutoriseeEnfantService.RemoveLinkPersonneAutoriseeEnfantAsync(2, 1));
        }

        [Fact]
        public async Task RemoveLinkPersonneAutoriseeEnfantAsync_WhenLinkDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeEnfantRepositoryMock
                .Setup(repo => repo.LinkExistsAsync(1, 2))
                .ReturnsAsync(false);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _linkPersonneAutoriseeEnfantService.RemoveLinkPersonneAutoriseeEnfantAsync(2, 1));

            // Assert
            exception.Message.ShouldBe("Le lien Personne Autorisée / Enfant n'existe pas.");
        }
    }
}