using AutoMapper;
using ChildcareManagementERP.Api.Dtos.V1;
using ChildcareManagementERP.Api.Infrastructure.Persistence.Models;
using ChildcareManagementERP.Api.Repositories;
using ChildcareManagementERP.Api.Services;
using Moq;
using Shouldly;

namespace ChildcareManagementERP.Api.UnitTests.Services
{
    public class FinancialInformationServiceTests
    {
        private readonly IFinancialInformationService _financialInformationService;
        private readonly Mock<IFinancialInformationRepository> _financialInformationRepositoryMock;
        private readonly Mock<IMapper> _mapperMock;

        public FinancialInformationServiceTests()
        {
            _financialInformationRepositoryMock = new Mock<IFinancialInformationRepository>();
            _mapperMock = new Mock<IMapper>();
            _financialInformationService = new FinancialInformationService(_financialInformationRepositoryMock.Object, _mapperMock.Object);
        }

        [Fact]
        public async Task GetAllFinancialInformationsAsync_WhenFinancialInformationsExist_ShouldReturnMappedDtoList()
        {
            // Arrange
            var financialInformations = new List<FinancialInformation>
            {
                new() { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) },
                new() { Id = 2, GuardianId = 200, StartDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) }
            };

            var financialInformationsDto = new List<FinancialInformationDto>
            {
                new() { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) },
                new() { Id = 2, GuardianId = 200, StartDate = new DateOnly(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day) }
            };

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(financialInformations);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<FinancialInformationDto>>(financialInformations))
                .Returns(financialInformationsDto);

            // Act
            var result = await _financialInformationService.GetAllFinancialInformationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.Count().ShouldBe(2);
            result.ShouldContain(e => e.GuardianId == 100);
        }

        [Fact]
        public async Task GetAllFinancialInformationsAsync_WhenNoFinancialInformation_ShouldReturnEmptyDtoList()
        {
            // Arrange
            var financialInformations = new List<FinancialInformation>();
            var financialInformationsDto = new List<FinancialInformationDto>();

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetAllAsync())
                .ReturnsAsync(financialInformations);

            _mapperMock
                .Setup(m => m.Map<IEnumerable<FinancialInformationDto>>(financialInformations))
                .Returns(financialInformationsDto);

            // Act
            var result = await _financialInformationService.GetAllFinancialInformationsAsync();

            // Assert
            result.ShouldNotBeNull();
            result.ShouldBeEmpty();
        }

        [Fact]
        public async Task GetFinancialInformationAsync_WhenFinancialInformationExists_ShouldReturnMappedDto()
        {
            // Arrange
            var financialInformation = new FinancialInformation { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };
            var financialInformationDto = new FinancialInformationDto { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(financialInformation);

            _mapperMock
                .Setup(m => m.Map<FinancialInformationDto>(financialInformation))
                .Returns(financialInformationDto);

            // Act
            var result = await _financialInformationService.GetFinancialInformationAsync(1);

            // Assert
            result.ShouldNotBeNull();
            result.GuardianId.ShouldBe(100);
        }

        [Fact]
        public async Task GetFinancialInformationAsync_WhenFinancialInformationDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            _financialInformationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(1))
                .ReturnsAsync(null as FinancialInformation);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _financialInformationService.GetFinancialInformationAsync(1));

            // Assert
            exception.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée.");
        }

        [Fact]
        public async Task CreateFinancialInformationAsync_WhenFinancialInformationIsCreated_ShouldReturnMappedDto()
        {
            // Arrange
            var newFinancialInformationDto = new CreateFinancialInformationDto { GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };
            var financialInformation = new FinancialInformation { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };
            var createdFinancialInformationDto = new FinancialInformationDto { Id = 1, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _mapperMock
                .Setup(m => m.Map<FinancialInformation>(newFinancialInformationDto))
                .Returns(financialInformation);

            _financialInformationRepositoryMock
                .Setup(repo => repo.AddAsync(financialInformation))
                .Returns(Task.CompletedTask);

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(financialInformation.Id))
                .ReturnsAsync(financialInformation);

            _mapperMock
                .Setup(m => m.Map<FinancialInformationDto>(financialInformation))
                .Returns(createdFinancialInformationDto);

            // Act
            var result = await _financialInformationService.CreateFinancialInformationAsync(newFinancialInformationDto);

            // Assert
            result.ShouldNotBeNull();
            result.GuardianId.ShouldBe(100);

            _financialInformationRepositoryMock.Verify(repo => repo.AddAsync(financialInformation), Times.Once);
        }

        [Fact]
        public async Task CreateFinancialInformationAsync_WhenMappingFails_ShouldReturnFail()
        {
            // Arrange
            var newFinancialInformationDto = new CreateFinancialInformationDto { GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _mapperMock
                .Setup(m => m.Map<FinancialInformation>(newFinancialInformationDto))
                .Returns((FinancialInformation)null!);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _financialInformationService.CreateFinancialInformationAsync(newFinancialInformationDto));

            // Assert
            exception.Message.ShouldBe("Erreur lors de la création de l'information financière : Le Mapping a échoué.");

            _financialInformationRepositoryMock.Verify(repo => repo.AddAsync(It.IsAny<FinancialInformation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFinancialInformationAsync_WhenFinancialInformationExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var financialInformation = new FinancialInformation { Id = id, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };
            var updateFinancialInformationDto = new UpdateFinancialInformationDto { Id = 1, GuardianId = 200, StartDate = new DateOnly(DateTime.Now.Year, 2, 1) };

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(financialInformation);

            _mapperMock
                .Setup(m => m.Map(updateFinancialInformationDto, financialInformation))
                .Returns(financialInformation);

            _financialInformationRepositoryMock
                .Setup(repo => repo.UpdateAsync(financialInformation))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _financialInformationService.UpdateFinancialInformationAsync(id, updateFinancialInformationDto));

            _financialInformationRepositoryMock.Verify(r => r.UpdateAsync(financialInformation), Times.Once);
        }

        [Fact]
        public async Task UpdateFinancialInformationAsync_WhenIdMismatch_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateFinancialInformationDto = new UpdateFinancialInformationDto { Id = 3, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _financialInformationService.UpdateFinancialInformationAsync(id, updateFinancialInformationDto));

            // Assert
            exception.Message.ShouldBe("L'identifiant de l'information financière ne correspond pas à celui de l'objet envoyé.");

            _financialInformationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<FinancialInformation>()), Times.Never);
        }

        [Fact]
        public async Task UpdateFinancialInformationAsync_WhenFinancialInformationDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;
            var updateFinancialInformationDto = new UpdateFinancialInformationDto { Id = id, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as FinancialInformation);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _financialInformationService.UpdateFinancialInformationAsync(id, updateFinancialInformationDto));

            // Assert
            exception.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée.");

            _financialInformationRepositoryMock.Verify(r => r.UpdateAsync(It.IsAny<FinancialInformation>()), Times.Never);
        }

        [Fact]
        public async Task DeleteFinancialInformationAsync_WhenFinancialInformationExists_ShouldReturnOk()
        {
            // Arrange
            var id = 1;
            var financialInformation = new FinancialInformation { Id = id, GuardianId = 100, StartDate = new DateOnly(DateTime.Now.Year, 1, 1) };

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(financialInformation);

            _financialInformationRepositoryMock
                .Setup(repo => repo.DeleteAsync(id))
                .Returns(Task.CompletedTask);

            // Act

            // Assert
            await Should.NotThrowAsync(async () => await _financialInformationService.DeleteFinancialInformationAsync(id));

            _financialInformationRepositoryMock.Verify(repo => repo.DeleteAsync(id), Times.Once);
        }

        [Fact]
        public async Task DeleteFinancialInformationAsync_WhenFinancialInformationDoesNotExist_ShouldReturnFail()
        {
            // Arrange
            var id = 1;

            _financialInformationRepositoryMock
                .Setup(repo => repo.GetByIdAsync(id))
                .ReturnsAsync(null as FinancialInformation);

            // Act
            var exception = await Should.ThrowAsync<Exception>(async () => await _financialInformationService.DeleteFinancialInformationAsync(id));

            // Assert
            exception.Message.ShouldBe("Aucune information financière correspondante n'a été trouvée.");

            _financialInformationRepositoryMock.Verify(repo => repo.DeleteAsync(It.IsAny<int>()), Times.Never);
        }
    }
}