using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;
using ChildcareManagementERP.Api.Services;
using Moq;
using Shouldly;

namespace ChildcareManagementERP.Api.UnitTests.Services
{
    public class ChildServiceTests
    {
        private readonly IChildService _childService;
        private readonly Mock<IChildRepository> _childRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public ChildServiceTests()
        {
            _childRepositoryMock = new Mock<IChildRepository>();
            _mapperMock = new Mock<IMapper>();
            _childService = new ChildService(_childRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllChildrenAsync_WhenChildrenExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var children = new List<Child>
            {
                new() { Id = 1, LastName = "Alice" },
                new() { Id = 2, LastName = "Bob" }
            };

            var childrenDto = new List<ChildDto>
            {
                new() { Id = 1, LastName = "Alice" },
                new() { Id = 2, LastName = "Bob" }
            };

            _childRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(children);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ChildDto>>(children))
                .Returns(childrenDto);

            // Act
            var result = await _childService.GetAllChildrenAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(r => r.LastName == "Alice");
        }

        [Fact]
        public async Task GetAllChildrenAsync_WhenNoChild_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var children = new List<Child>();
            var childrenDto = new List<ChildDto>();

            _childRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(children);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<ChildDto>>(children))
                .Returns(childrenDto);

            // Act
            var result = await _childService.GetAllChildrenAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildAsync_WhenChildExists_ShouldReturnMappedDto()
        {
            // Arrange
            var child = new Child { Id = 1, LastName = "Alice" };
            var childDto = new ChildDto { Id = 1, LastName = "Alice" };

            _childRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildDto>(child))
                .Returns(childDto);

            // Act
            var result = await _childService.GetChildAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
        }

        [Fact]
        public async Task GetChildAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _childRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as Child);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.GetChildAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetChildWithGuardiansAsync_WhenChildWithGuardiansExists_ShouldReturnMappedDto()
        {
            // Arrange
            var child = new Child
            {
                Id = 1,
                LastName = "Alice",
                GuardianChildren = new List<GuardianChild>
                {
                    new() { Guardian = new Guardian { Id = 1, LastName = "Bob" } },
                    new() { Guardian = new Guardian { Id = 2, LastName = "Charlie" } }
                }
            };

            var childWithGuardiansDto = new ChildWithGuardiansDto
            {
                Id = 1,
                LastName = "Alice",
                Guardians = new List<GuardianDto>
                {
                    new() { Id = 1, LastName = "Bob" },
                    new() { Id = 2, LastName = "Charlie" }
                }
            };

            _childRepositoryMock
                .Setup(repo => repo.GetWithGuardiansAsync(1))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildWithGuardiansDto>(child))
                .Returns(childWithGuardiansDto);

            // Act
            var result = await _childService.GetChildWithGuardiansAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldNotBeEmpty();
            result.Guardians.Count.ShouldBe(2);
            result.Guardians.ShouldContain(r => r.LastName == "Bob");
        }

        [Fact]
        public async Task GetChildWithGuardiansAsync_WhenGuardiansListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var child = new Child
            {
                Id = 1,
                LastName = "Alice",
                GuardianChildren = new List<GuardianChild>()
            };

            var childWithGuardiansDto = new ChildWithGuardiansDto
            {
                Id = 1,
                LastName = "Alice",
                Guardians = new List<GuardianDto>()
            };

            _childRepositoryMock
                .Setup(repo => repo.GetWithGuardiansAsync(1))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildWithGuardiansDto>(child))
                .Returns(childWithGuardiansDto);

            // Act
            var result = await _childService.GetChildWithGuardiansAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Alice");
            result.Guardians.ShouldNotBeNull();
            result.Guardians.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildWithGuardiansAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _childRepositoryMock
                .Setup(repo => repo.GetWithGuardiansAsync(1))
                .ReturnsAsync(null as Child);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.GetChildWithGuardiansAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetChildWithAuthorizedPeopleAsync_WhenChildWithAuthorizedPeopleExists_ShouldReturnMappedDto()
        {
            // Arrange
            var child = new Child
            {
                Id = 1,
                LastName = "Alice",
                AuthorizedPersonChildren = new List<AuthorizedPersonChild>
                {
                    new() { AuthorizedPerson = new AuthorizedPerson { Id = 1, LastName = "Bob" } },
                    new() { AuthorizedPerson = new AuthorizedPerson { Id = 2, LastName = "Charlie" } }
                }
            };

            var childWithAuthorizedPeopleDto = new ChildWithAuthorizedPeopleDto
            {
                Id = 1,
                LastName = "Alice",
                AuthorizedPeople = new List<AuthorizedPersonDto>
                {
                    new() { Id = 1, LastName = "Bob" },
                    new() { Id = 2, LastName = "Charlie" }
                }
            };
            _childRepositoryMock
                .Setup(repo => repo.GetWithAuthorizedPeopleAsync(1))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildWithAuthorizedPeopleDto>(child))
                .Returns(childWithAuthorizedPeopleDto);

            // Act
            var result = await _childService.GetChildWithAuthorizedPeopleAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldNotBeEmpty();
            result.AuthorizedPeople.Count.ShouldBe(2);
            result.AuthorizedPeople.ShouldContain(r => r.LastName == "Bob");
        }

        [Fact]
        public async Task GetChildWithAuthorizedPeopleAsync_WhenAuthorizedPeopleListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var child = new Child
            {
                Id = 1,
                LastName = "Alice",
                AuthorizedPersonChildren = new List<AuthorizedPersonChild>()
            };

            var childWithAuthorizedPeopleDto = new ChildWithAuthorizedPeopleDto
            {
                Id = 1,
                LastName = "Alice",
                AuthorizedPeople = new List<AuthorizedPersonDto>()
            };

            _childRepositoryMock
                .Setup(repo => repo.GetWithAuthorizedPeopleAsync(1))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildWithAuthorizedPeopleDto>(child))
                .Returns(childWithAuthorizedPeopleDto);

            // Act
            var result = await _childService.GetChildWithAuthorizedPeopleAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Alice");
            result.AuthorizedPeople.ShouldNotBeNull();
            result.AuthorizedPeople.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildWithAuthorizedPeopleAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _childRepositoryMock
                .Setup(repo => repo.GetWithAuthorizedPeopleAsync(1))
                .ReturnsAsync(null as Child);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.GetChildWithAuthorizedPeopleAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task GetChildWithAdditionalDatasAsync_WhenChildWithAdditionalDatasExists_ShouldReturnMappedDto()
        {
            // Arrange
            var child = new Child
            {
                Id = 1,
                LastName = "Alice",
                AdditionalData = new List<AdditionalDatum>
                {
                    new() { Id = 1, ParamValue = "A" },
                    new() { Id = 2, ParamValue = "B" }
                }
            };

            var childWithAdditionalDatasDto = new ChildWithAdditionalDatasDto
            {
                Id = 1,
                LastName = "Alice",
                AdditionalDatas = new List<AdditionalDataDto>
                {
                    new() { Id = 1, ParamValue = "A" },
                    new() { Id = 2, ParamValue = "B" }
                }
            };

            _childRepositoryMock
                .Setup(repo => repo.GetWithAdditionalDatasAsync(1))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildWithAdditionalDatasDto>(child))
                .Returns(childWithAdditionalDatasDto);

            // Act
            var result = await _childService.GetChildWithAdditionalDatasAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldNotBeEmpty();
            result.AdditionalDatas.Count.ShouldBe(2);
            result.AdditionalDatas.ShouldContain(r => r.ParamValue == "A");
        }

        [Fact]
        public async Task GetChildWithAdditionalDatasAsync_WhenAdditionalDatasListIsEmpty_ShouldReturnMappedDto()
        {
            // Arrange
            var child = new Child
            {
                Id = 1,
                LastName = "Alice",
                AdditionalData = new List<AdditionalDatum>()
            };

            var childWithAdditionalDatasDto = new ChildWithAdditionalDatasDto
            {
                Id = 1,
                LastName = "Alice",
                AdditionalDatas = new List<AdditionalDataDto>()
            };

            _childRepositoryMock
                .Setup(repo => repo.GetWithAdditionalDatasAsync(1))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildWithAdditionalDatasDto>(child))
                .Returns(childWithAdditionalDatasDto);

            // Act
            var result = await _childService.GetChildWithAdditionalDatasAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.Id.ShouldBe(1);
            result.LastName.ShouldBe("Alice");
            result.AdditionalDatas.ShouldNotBeNull();
            result.AdditionalDatas.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetChildWithAdditionalDatasAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _childRepositoryMock
                .Setup(repo => repo.GetWithAdditionalDatasAsync(1))
                .ReturnsAsync(null as Child);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.GetChildWithAdditionalDatasAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");
        }

        [Fact]
        public async Task CreateChildAsync_WhenChildIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newChildDto = new CreateChildDto { LastName = "Alice" };
            var child = new Child { Id = 1, LastName = "Alice" };
            var createdChildDto = new ChildDto { Id = 1, LastName = "Alice" };

            _mapperMock
                .Setup(m => m.Map<Child>(newChildDto))
                .Returns(child);

            _childRepositoryMock
                .Setup(repo => repo.AddAsync(child))
                .Returns(Task.CompletedTask);

            _childRepositoryMock
                .Setup(repo => repo.GetByIdAsync(child.Id))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map<ChildDto>(child))
                .Returns(createdChildDto);

            // Act
            var result = await _childService.CreateChildAsync(newChildDto);

            // Assert
            result.ShouldNotBeNull();
            result.LastName.ShouldBe("Alice");

            _childRepositoryMock.Verify(repo => repo.AddAsync(child), Times.Once);
        }

        [Fact]
        public async Task CreateChildAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newChildDto = new CreateChildDto { LastName = "Alice" };

            _mapperMock
                .Setup(m => m.Map<Child>(newChildDto))
                .Returns((Child)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.CreateChildAsync(newChildDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de l'enfant : Le Mapping a échoué.");

            _childRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<Child>()), Times.Never);
        }

        [Fact]
        public async Task UpdateChildAsync_WhenChildExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var updateChildDto = new UpdateChildDto { Id = 1, LastName = "Alice" };
            var child = new Child { Id = id, LastName = "Alice" };

            _childRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(child);

            _mapperMock
                .Setup(m => m.Map(updateChildDto, child))
                .Returns(child);

            _childRepositoryMock
                .Setup(repo => repo.UpdateAsync(child))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _childService.UpdateChildAsync(id, updateChildDto));

            _childRepositoryMock.Verify(r => r.UpdateAsync(child), Times.Once);
        }

        [Fact]
        public async Task UpdateChildAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateChildDto = new UpdateChildDto { Id = 2, LastName = "Alice" };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.UpdateChildAsync(id, updateChildDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de l'enfant ne correspond pas à celui de l'objet envoyé.");

            _childRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Child>()), Times.Never);
        }

        [Fact]
        public async Task UpdateChildAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateChildDto = new UpdateChildDto { Id = id, LastName = "Alice" };

            _childRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Child);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.UpdateChildAsync(id, updateChildDto));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");

            _childRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<Child>()), Times.Never);
        }

        [Fact]
        public async Task DeleteChildAsync_WhenChildExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var child = new Child { Id = id, LastName = "Alice" };

            _childRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(child);

            _childRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _childService.DeleteChildAsync(id));

            _childRepositoryMock.Verify(r => r.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteChildAsync_WhenChildDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _childRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as Child);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _childService.DeleteChildAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucun enfant correspondant n'a été trouvé.");

            _childRepositoryMock.Verify(r => r.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}