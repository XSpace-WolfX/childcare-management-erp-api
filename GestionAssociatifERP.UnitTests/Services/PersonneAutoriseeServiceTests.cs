using AutoMapper;
using GestionAssociatifERP.Dtos.V1;
using GestionAssociatifERP.Models;
using GestionAssociatifERP.Repositories;
using GestionAssociatifERP.Services;
using Moq;
using Shouldly;

namespace GestionAssociatifERP.UnitTests.Services
{
    public class PersonneAutoriseeServiceTests
    {
        private readonly IAuthorizedPersonService _personneAutoriseeService;
        private readonly Mock<IAuthorizedPersonRepository> _personneAutoriseeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public PersonneAutoriseeServiceTests()
        {
            _personneAutoriseeRepositoryMock = new Mock<IAuthorizedPersonRepository>();
            _mapperMock = new Mock<IMapper>();
            _personneAutoriseeService = new AuthorizedPersonService(_personneAutoriseeRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllPersonnesAutoriseesAsync_WhenPersonnesAutoriseesExists_ShouldReturnMappedDtoList()
        {
            // Arrange
            var personnesAutorisees = new List<PersonneAutorisee>
            {
                new() { Id = 1, Prenom = "John", Nom = "Doe" },
                new() { Id = 2, Prenom = "Jane", Nom = "Doe" }
            };

            var personnesAutoriseesDto = new List<AuthorizedPersonDto>
            {
                new() { Id = 1, FirstName = "John", LastName = "Doe" },
                new() { Id = 2, FirstName = "Jane", LastName = "Doe" }
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(personnesAutorisees);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AuthorizedPersonDto>>(personnesAutorisees))
                .Returns(personnesAutoriseesDto);

            // Act
            var result = await _personneAutoriseeService.GetAllAuthorizedPeopleAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.LastName == "Doe");
        }

        [Fact]
        public async Task GetAllPersonnesAutoriseesAsync_WhenNoPersonnesAutorisees_ShouldReturnEmptyList()
        {
            // Arrange
            var personnesAutorisees = new List<PersonneAutorisee>();
            var personnesAutoriseesDto = new List<AuthorizedPersonDto>();

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(personnesAutorisees);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<AuthorizedPersonDto>>(personnesAutorisees))
                .Returns(personnesAutoriseesDto);

            // Act
            var result = await _personneAutoriseeService.GetAllAuthorizedPeopleAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonneAutoriseeAsync_WhenPersonneAutoriseeExists_ShouldReturnMappedDto()
        {
            // Arrange
            var personneAutorisee = new PersonneAutorisee { Id = 1, Prenom = "John", Nom = "Doe" };
            var personneAutoriseeDto = new AuthorizedPersonDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonDto>(personneAutorisee))
                .Returns(personneAutoriseeDto);

            // Act
            var result = await _personneAutoriseeService.GetAuthorizedPersonAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
        }

        [Fact]
        public async Task GetPersonneAutoriseeAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as PersonneAutorisee);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.GetAuthorizedPersonAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task GetPersonneAutoriseeWithEnfantsAsync_WhenPersonneAutoriseeWithEnfantsExists_ShouldReturnMappedDto()
        {
            // Arrange
            var personneAutorisee = new PersonneAutorisee
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                PersonneAutoriseeEnfants = new List<PersonneAutoriseeEnfant>
                {
                    new() { Enfant = new Enfant { Id = 1, Prenom = "Jean", Nom = "Doe" } },
                    new() { Enfant = new Enfant { Id = 2, Prenom = "Jade", Nom = "Doe" } }
                }
            };

            var personneAutoriseeWithEnfantsDto = new AuthorizedPersonWithChildrenDto
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

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonWithChildrenDto>(personneAutorisee))
                .Returns(personneAutoriseeWithEnfantsDto);

            // Act
            var result = await _personneAutoriseeService.GetAuthorizedPersonWithChildrenAsync(1);

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
        public async Task GetPersonneAutoriseeWithEnfantsAsync_WhenEnfantsListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var personneAutorisee = new PersonneAutorisee
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                PersonneAutoriseeEnfants = new List<PersonneAutoriseeEnfant>()
            };

            var personneAutoriseeWithEnfantsDto = new AuthorizedPersonWithChildrenDto
            {
                Id = 1,
                FirstName = "John",
                LastName = "Doe",
                Children = new List<ChildDto>()
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonWithChildrenDto>(personneAutorisee))
                .Returns(personneAutoriseeWithEnfantsDto);

            // Act
            var result = await _personneAutoriseeService.GetAuthorizedPersonWithChildrenAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");
            result.Children.ShouldNotBeNull();
            result.Children.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonneAutoriseeWithEnfantsAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetWithChildrenAsync(1))
                .ReturnsAsync(null as PersonneAutorisee);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.GetAuthorizedPersonWithChildrenAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreatePersonneAutoriseeAsync_WhenPersonneAutoriseeIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newPersonneAutoriseeDto = new CreateAuthorizedPersonDto { FirstName = "John", LastName = "Doe" };
            var personneAutorisee = new PersonneAutorisee { Id = 1, Prenom = "John", Nom = "Doe" };
            var createdPersonneAutoriseeDto = new AuthorizedPersonDto { Id = 1, FirstName = "John", LastName = "Doe" };

            _mapperMock
                .Setup(m => m.Map<PersonneAutorisee>(newPersonneAutoriseeDto))
                .Returns(personneAutorisee);

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.AddAsync(personneAutorisee))
                .Returns(Task.CompletedTask);

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(personneAutorisee.Id))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map<AuthorizedPersonDto>(personneAutorisee))
                .Returns(createdPersonneAutoriseeDto);

            // Act
            var result = await _personneAutoriseeService.CreateAuthorizedPersonAsync(newPersonneAutoriseeDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.FirstName.ShouldBe("John");
            result.LastName.ShouldBe("Doe");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.AddAsync(personneAutorisee), Times.Once);
        }

        [Fact]
        public async Task CreatePersonneAutoriseeAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeDto = new CreateAuthorizedPersonDto { FirstName = "John", LastName = "Doe" };
            
            _mapperMock
                .Setup(m => m.Map<PersonneAutorisee>(newPersonneAutoriseeDto))
                .Returns((PersonneAutorisee)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.CreateAuthorizedPersonAsync(newPersonneAutoriseeDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de la personne autorisée : Le Mapping a échoué.");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<PersonneAutorisee>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePersonneAutoriseeAsync_WhenPersonneAutoriseeExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updatePersonneAutoriseeDto = new UpdateAuthorizedPersonDto { Id = 1, FirstName = "John", LastName = "Doe" };
            var personneAutorisee = new PersonneAutorisee { Id = id, Prenom = "John", Nom = "Doe" };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map(updatePersonneAutoriseeDto, personneAutorisee))
                .Returns(personneAutorisee);

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.UpdateAsync(personneAutorisee))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _personneAutoriseeService.UpdateAuthorizedPersonAsync(id, updatePersonneAutoriseeDto));

            _personneAutoriseeRepositoryMock.Verify(repo => repo.UpdateAsync(personneAutorisee), Times.Once);
        }

        [Fact]
        public async Task UpdatePersonneAutoriseeAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updatePersonneAutoriseeDto = new UpdateAuthorizedPersonDto { Id = 2, FirstName = "John", LastName = "Doe" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.UpdateAuthorizedPersonAsync(id, updatePersonneAutoriseeDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de la personne autorisée ne correspond pas à celui de l'objet envoyé.");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<PersonneAutorisee>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePersonneAutoriseeAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updatePersonneAutoriseeDto = new UpdateAuthorizedPersonDto { Id = id, FirstName = "John", LastName = "Doe" };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as PersonneAutorisee);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.UpdateAuthorizedPersonAsync(id, updatePersonneAutoriseeDto));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<PersonneAutorisee>()), Times.Never);
        }

        [Fact]
        public async Task DeletePersonneAutoriseeAsync_WhenPersonneAutoriseeExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var personneAutorisee = new PersonneAutorisee { Id = id, Prenom = "John", Nom = "Doe" };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(personneAutorisee);

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _personneAutoriseeService.DeleteAuthorizedPersonAsync(id));

            _personneAutoriseeRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeletePersonneAutoriseeAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as PersonneAutorisee);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.DeleteAuthorizedPersonAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}