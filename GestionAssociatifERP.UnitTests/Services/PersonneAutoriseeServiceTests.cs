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
        private readonly IPersonneAutoriseeService _personneAutoriseeService;
        private readonly Mock<IPersonneAutoriseeRepository> _personneAutoriseeRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public PersonneAutoriseeServiceTests()
        {
            _personneAutoriseeRepositoryMock = new Mock<IPersonneAutoriseeRepository>();
            _mapperMock = new Mock<IMapper>();
            _personneAutoriseeService = new PersonneAutoriseeService(_personneAutoriseeRepositoryMock.Object, _mapperMock.Object);
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

            var personnesAutoriseesDto = new List<PersonneAutoriseeDto>
            {
                new() { Id = 1, Prenom = "John", Nom = "Doe" },
                new() { Id = 2, Prenom = "Jane", Nom = "Doe" }
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(personnesAutorisees);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<PersonneAutoriseeDto>>(personnesAutorisees))
                .Returns(personnesAutoriseesDto);

            // Act
            var result = await _personneAutoriseeService.GetAllPersonnesAutoriseesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.Nom == "Doe");
        }

        [Fact]
        public async Task GetAllPersonnesAutoriseesAsync_WhenNoPersonnesAutorisees_ShouldReturnEmptyList()
        {
            // Arrange
            var personnesAutorisees = new List<PersonneAutorisee>();
            var personnesAutoriseesDto = new List<PersonneAutoriseeDto>();

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(personnesAutorisees);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<PersonneAutoriseeDto>>(personnesAutorisees))
                .Returns(personnesAutoriseesDto);

            // Act
            var result = await _personneAutoriseeService.GetAllPersonnesAutoriseesAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonneAutoriseeAsync_WhenPersonneAutoriseeExists_ShouldReturnMappedDto()
        {
            // Arrange
            var personneAutorisee = new PersonneAutorisee { Id = 1, Prenom = "John", Nom = "Doe" };
            var personneAutoriseeDto = new PersonneAutoriseeDto { Id = 1, Prenom = "John", Nom = "Doe" };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeDto>(personneAutorisee))
                .Returns(personneAutoriseeDto);

            // Act
            var result = await _personneAutoriseeService.GetPersonneAutoriseeAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");
        }

        [Fact]
        public async Task GetPersonneAutoriseeAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as PersonneAutorisee);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.GetPersonneAutoriseeAsync(1));

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

            var personneAutoriseeWithEnfantsDto = new PersonneAutoriseeWithEnfantsDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                Enfants = new List<EnfantDto>
                {
                    new() { Id = 1, Prenom = "Jean", Nom = "Doe" },
                    new() { Id = 2, Prenom = "Jade", Nom = "Doe" }
                }
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetWithEnfantsAsync(1))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeWithEnfantsDto>(personneAutorisee))
                .Returns(personneAutoriseeWithEnfantsDto);

            // Act
            var result = await _personneAutoriseeService.GetPersonneAutoriseeWithEnfantsAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");
            result.Enfants.ShouldNotBeNull();
            result.Enfants.ShouldNotBeEmpty();
            result.Enfants.Count.ShouldBe(2);
            result.Enfants.ShouldContain(e => e.Prenom == "Jean");
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

            var personneAutoriseeWithEnfantsDto = new PersonneAutoriseeWithEnfantsDto
            {
                Id = 1,
                Prenom = "John",
                Nom = "Doe",
                Enfants = new List<EnfantDto>()
            };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetWithEnfantsAsync(1))
                .ReturnsAsync(personneAutorisee);

            _mapperMock
                .Setup(m => m.Map<PersonneAutoriseeWithEnfantsDto>(personneAutorisee))
                .Returns(personneAutoriseeWithEnfantsDto);

            // Act
            var result = await _personneAutoriseeService.GetPersonneAutoriseeWithEnfantsAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");
            result.Enfants.ShouldNotBeNull();
            result.Enfants.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetPersonneAutoriseeWithEnfantsAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetWithEnfantsAsync(1))
                .ReturnsAsync(null as PersonneAutorisee);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.GetPersonneAutoriseeWithEnfantsAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreatePersonneAutoriseeAsync_WhenPersonneAutoriseeIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newPersonneAutoriseeDto = new CreatePersonneAutoriseeDto { Prenom = "John", Nom = "Doe" };
            var personneAutorisee = new PersonneAutorisee { Id = 1, Prenom = "John", Nom = "Doe" };
            var createdPersonneAutoriseeDto = new PersonneAutoriseeDto { Id = 1, Prenom = "John", Nom = "Doe" };

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
                .Setup(m => m.Map<PersonneAutoriseeDto>(personneAutorisee))
                .Returns(createdPersonneAutoriseeDto);

            // Act
            var result = await _personneAutoriseeService.CreatePersonneAutoriseeAsync(newPersonneAutoriseeDto);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.Prenom.ShouldBe("John");
            result.Nom.ShouldBe("Doe");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.AddAsync(personneAutorisee), Times.Once);
        }

        [Fact]
        public async Task CreatePersonneAutoriseeAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newPersonneAutoriseeDto = new CreatePersonneAutoriseeDto { Prenom = "John", Nom = "Doe" };
            
            _mapperMock
                .Setup(m => m.Map<PersonneAutorisee>(newPersonneAutoriseeDto))
                .Returns((PersonneAutorisee)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.CreatePersonneAutoriseeAsync(newPersonneAutoriseeDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de la personne autorisée : Le Mapping a échoué.");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<PersonneAutorisee>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePersonneAutoriseeAsync_WhenPersonneAutoriseeExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updatePersonneAutoriseeDto = new UpdatePersonneAutoriseeDto { Id = 1, Prenom = "John", Nom = "Doe" };
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
            await Should.NotThrowAsync(async () => await _personneAutoriseeService.UpdatePersonneAutoriseeAsync(id, updatePersonneAutoriseeDto));

            _personneAutoriseeRepositoryMock.Verify(repo => repo.UpdateAsync(personneAutorisee), Times.Once);
        }

        [Fact]
        public async Task UpdatePersonneAutoriseeAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updatePersonneAutoriseeDto = new UpdatePersonneAutoriseeDto { Id = 2, Prenom = "John", Nom = "Doe" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.UpdatePersonneAutoriseeAsync(id, updatePersonneAutoriseeDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de la personne autorisée ne correspond pas à celui de l'objet envoyé.");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.UpdateAsync(It.IsAny<PersonneAutorisee>()), Times.Never);
        }

        [Fact]
        public async Task UpdatePersonneAutoriseeAsync_WhenPersonneAutoriseeDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updatePersonneAutoriseeDto = new UpdatePersonneAutoriseeDto { Id = id, Prenom = "John", Nom = "Doe" };

            _personneAutoriseeRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as PersonneAutorisee);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.UpdatePersonneAutoriseeAsync(id, updatePersonneAutoriseeDto));

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
            await Should.NotThrowAsync(async () => await _personneAutoriseeService.DeletePersonneAutoriseeAsync(id));

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
            var exception = await Should.ThrowAsync<Exception>(async () => await _personneAutoriseeService.DeletePersonneAutoriseeAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune personne autorisée correspondante n'a été trouvée.");

            _personneAutoriseeRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}