using FluentAssertions;
using Moq;
using TasksManagement.Application.Services;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;

namespace TasksManagement.Tests.Services
{
    public class TaskHistoryServiceTests
    {
        private readonly Mock<ITaskHistoryRepository> _mockTaskHistoryRepository;

        public TaskHistoryServiceTests()
        {
            _mockTaskHistoryRepository = new Mock<ITaskHistoryRepository>();
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Success_When_History_Is_Created()
        {
            // Arrange
            var taskHistory = new TaskHistory("Changed status to Complete", Guid.NewGuid(), Guid.NewGuid());
            _mockTaskHistoryRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<TaskHistory>()))
                .ReturnsAsync(Result.Success());

            var service = new TaskHistoryService(_mockTaskHistoryRepository.Object);

            // Act
            var result = await service.CreateAsync(taskHistory);

            // Assert
            result.IsValid.Should().BeTrue();
            _mockTaskHistoryRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskHistory>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
        {
            // Arrange
            var taskHistory = new TaskHistory("Changed status to In Progress", Guid.NewGuid(), Guid.NewGuid());
            _mockTaskHistoryRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<TaskHistory>()))
                .ThrowsAsync(new Exception("Falha ao gravar histórico da tarefa."));

            var service = new TaskHistoryService(_mockTaskHistoryRepository.Object);

            // Act
            var result = await service.CreateAsync(taskHistory);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("Falha ao gravar histórico da tarefa.");
            _mockTaskHistoryRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskHistory>()), Times.Once);
        }

        [Fact]
        public async Task CreateAsync_Should_Return_Failure_When_Repository_Throws_Exception()
        {
            // Arrange
            var taskHistory = new TaskHistory("Changed status to In Progress", Guid.NewGuid(), Guid.NewGuid());
            _mockTaskHistoryRepository
                .Setup(repo => repo.CreateAsync(It.IsAny<TaskHistory>()))
                .ThrowsAsync(new Exception("Erro desconhecido."));

            var service = new TaskHistoryService(_mockTaskHistoryRepository.Object);

            // Act
            var result = await service.CreateAsync(taskHistory);

            // Assert
            result.IsValid.Should().BeFalse();
            result.Message.Should().Contain("Falha ao gravar histórico da tarefa.");
            _mockTaskHistoryRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskHistory>()), Times.Once);
        }
    }
}