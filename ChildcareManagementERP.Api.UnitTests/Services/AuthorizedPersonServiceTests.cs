using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;
using ChildcareManagementERP.Api.Services;
using Moq;
using Shouldly;

namespace ChildcareManagementERP.Api.UnitTests.Services
{
    public class AuthorizedPersonServiceTests
    {
        private readonly IAuthorizedPersonService _authorizedPersonService;
        private readonly Mock<IAuthorizedPersonRepository> _authorizedPersonRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public AuthorizedPersonServiceTests()
        {
            _authorizedPersonRepositoryMock = new Mock<IAuthorizedPersonRepository>();
            _mapperMock = new Mock<IMapper>();
            _authorizedPersonService = new AuthorizedPersonService(_authorizedPersonRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllAuthorizedPersonsAsync_WhenAuthorizedPersonsExists_ShouldReturnMappedDtoList()
        {
            // Arrange
            var authorizedPersons = new List<AuthorizedPerson>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };

            var authorizedPersonsDto = new List<AuthorizedPersonDto>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(authorizedPersons);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AuthorizedPersonDto>>(authorizedPersons))
                .Returns(authorizedPersonsDto);

            // Act
            var result = await _authorizedPersonService.GetAllAuthorizedPeopleAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.LastName == "Doe");
        }

        [Fact]
        public async Task GetAllAuthorizedPersonsAsync_WhenNoAuthorizedPerson_ShouldReturnEmptyList()
        {
            // Arrange
            var authorizedPersons = new List<AuthorizedPerson>();
            var authorizedPersonsDto = new List<AuthorizedPersonDto>();

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(authorizedPersons);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AuthorizedPersonDto>>(authorizedPersons))
                .Returns(authorizedPersonsDto);

            // Act
            var result = await _authorizedPersonService.GetAllAuthorizedPeopleAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAuthorizedPersonAsync_WhenAuthorizedPersonExists_ShouldReturnMappedDto()
        {
            // Arrange
            var authorizedPerson = new AuthorizedPerson { Id = 1, FirstName = "John", LastName = "Doe" };
            var authorizedPersonDto = new AuthorizedPersonDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(authorizedPerson);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonDto>(authorizedPerson))
                .Returns(authorizedPersonDto);

            // Act
            var result = await _authorizedPersonService.GetAuthorizedPersonAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
        }

        [Fact]
        public async Task GetAuthorizedPersonAsync_WhenAuthorizedPersonDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as AuthorizedPerson);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _authorizedPersonService.GetAuthorizedPersonAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task GetAuthorizedPersonWithChildrenAsync_WhenAuthorizedPersonWithChildrenExists_ShouldReturnMappedDto()
        {
            // Arrange
            var authorizedPerson = new AuthorizedPerson
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                AuthorizedPersonChildren = new List<AuthorizedPersonChild>
                {
                    new() { Child = new Child { Id = 1, FirstName = "Jean", LastName = "Doe" } },
                    new() { Child = new Child { Id = 2, FirstName = "Jade", LastName = "Doe" } }
                }
            };

            var authorizedPersonWithChildrenDto = new AuthorizedPersonWithChildrenDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Children = new List<ChildDto>
                {
                    new() { Id = 1, FirstName = "Jean", LastName = "Doe" },
                    new() { Id = 2, FirstName = "Jade", LastName = "Doe" }
                }
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(authorizedPerson);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonWithChildrenDto>(authorizedPerson))
                .Returns(authorizedPersonWithChildrenDto);

            // Act
            var result = await _authorizedPersonService.GetAuthorizedPersonWithChildrenAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.Children.ShouldNotBeNull();
            result.Children.ShouldNotBeEmpty();
            result.Children.Count.ShouldBe(2);
            result.Children.ShouldContain(e => e.FirstName == "Jean");
        }

        [Fact]
        public async Task GetAuthorizedPersonWithChildrenAsync_WhenChildListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var authorizedPerson = new AuthorizedPerson
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                AuthorizedPersonChildren = new List<AuthorizedPersonChild>()
            };

            var authorizedPersonWithChildrenDto = new AuthorizedPersonWithChildrenDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Children = new List<ChildDto>()
            };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(authorizedPerson);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonWithChildrenDto>(authorizedPerson))
                .Returns(authorizedPersonWithChildrenDto);

            // Act
            var result = await _authorizedPersonService.GetAuthorizedPersonWithChildrenAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.Children.ShouldNotBeNull();
            result.Children.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetAuthorizedPersonWithChildrenAsync_WhenAuthorizedPersonDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(null as AuthorizedPerson);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _authorizedPersonService.GetAuthorizedPersonWithChildrenAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateAuthorizedPersonAsync_WhenAuthorizedPersonIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newAuthorizedPersonDto = new CreateAuthorizedPersonDto { FirstName = "John", LastName = "Doe" };
            var authorizedPerson = new AuthorizedPerson { Id = 1, FirstName = "John", LastName = "Doe" };
            var createdAuthorizedPersonDto = new AuthorizedPersonDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _mapperMock
                .Setup(m => m.Map<AuthorizedPerson>(newAuthorizedPersonDto))
                .Returns(authorizedPerson);

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.AddAsync(authorizedPerson))
                .Returns(Task.CompletedTask);

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetByIdAsync(authorizedPerson.Id))
                .ReturnsAsync(authorizedPerson);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonDto>(authorizedPerson))
                .Returns(createdAuthorizedPersonDto);

            // Act
            var result = await _authorizedPersonService.CreateAuthorizedPersonAsync(newAuthorizedPersonDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");

            _authorizedPersonRepositoryMock.Verify(repo => repo.AddAsync(authorizedPerson), Times.Once);
        }

        [Fact]
        public async Task CreateAuthorizedPersonAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newAuthorizedPersonDto = new CreateAuthorizedPersonDto { FirstName = "John", LastName = "Doe" };
            
            _mapperMock
                .Setup(m => m.Map<AuthorizedPerson>(newAuthorizedPersonDto))
                .Returns((AuthorizedPerson)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _authorizedPersonService.CreateAuthorizedPersonAsync(newAuthorizedPersonDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de la personne autorisée : Le Mapping a échoué.");

            _authorizedPersonRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<AuthorizedPerson>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAuthorizedPersonAsync_WhenAuthorizedPersonExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateAuthorizedPersonDto = new UpdateAuthorizedPersonDto { Id = 1, FirstName = "John", LastName = "Doe" };
            var authorizedPerson = new AuthorizedPerson { Id = id, FirstName = "John", LastName = "Doe" };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(authorizedPerson);

            _mapperMock
                .Setup(m => m.Map(updateAuthorizedPersonDto, authorizedPerson))
                .Returns(authorizedPerson);

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.UpdateAsync(authorizedPerson))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _authorizedPersonService.UpdateAuthorizedPersonAsync(id, updateAuthorizedPersonDto));

            _authorizedPersonRepositoryMock.Verify(repo => repo.UpdateAsync(authorizedPerson), Times.Once);
        }

        [Fact]
        public async Task UpdateAuthorizedPersonAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateAuthorizedPersonDto = new UpdateAuthorizedPersonDto { Id = 2, FirstName = "John", LastName = "Doe" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _authorizedPersonService.UpdateAuthorizedPersonAsync(id, updateAuthorizedPersonDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de la personne autorisée ne correspond pas à celui de l'objet envoyé.");

            _authorizedPersonRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<AuthorizedPerson>()), Times.Never);
        }

        [Fact]
        public async Task UpdateAuthorizedPersonAsync_WhenAuthorizedPersonDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateAuthorizedPersonDto = new UpdateAuthorizedPersonDto { Id = id, FirstName = "John", LastName = "Doe" };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as AuthorizedPerson);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _authorizedPersonService.UpdateAuthorizedPersonAsync(id, updateAuthorizedPersonDto));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");

            _authorizedPersonRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<AuthorizedPerson>()), Times.Never);
        }

        [Fact]
        public async Task DeleteAuthorizedPersonAsync_WhenAuthorizedPersonExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var authorizedPerson = new AuthorizedPerson { Id = id, FirstName = "John", LastName = "Doe" };

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(authorizedPerson);

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _authorizedPersonService.DeleteAuthorizedPersonAsync(id));

            _authorizedPersonRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteAuthorizedPersonAsync_WhenAuthorizedPersonDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _authorizedPersonRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as AuthorizedPerson);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _authorizedPersonService.DeleteAuthorizedPersonAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");

            _authorizedPersonRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}