using FluentAssertions;
using MongoDB.Driver;
using Moq;
using TasksManagement.Domain.Entities;
using TasksManagement.Infrastructure.Database;
using TasksManagement.Infrastructure.Repositories;

namespace TasksManagement.Tests.Repositories;
public class TaskHistoryRepositoryTests
{
    private readonly Mock<IMongoCollection<TaskHistory>> _mockTaskHistoryCollection;
    private readonly Mock<IMongoDbContext> _mockMongoDbContext;
    private readonly TaskHistoryRepository _taskHistoryRepository;

    public TaskHistoryRepositoryTests()
    {
        _mockTaskHistoryCollection = new Mock<IMongoCollection<TaskHistory>>();

        _mockMongoDbContext = new Mock<IMongoDbContext>();
        _mockMongoDbContext.Setup(m => m.TaskHistories).Returns(_mockTaskHistoryCollection.Object);

        _taskHistoryRepository = new TaskHistoryRepository(_mockMongoDbContext.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Insert_TaskHistory_Successfully()
    {
        // Arrange
        var taskHistory = new TaskHistory("Tarefa atualizada", Guid.NewGuid(), Guid.NewGuid());

        _mockTaskHistoryCollection
            .Setup(x => x.InsertOneAsync(It.IsAny<TaskHistory>(), null, default))
            .Returns(Task.CompletedTask);

        // Act
        var result = await _taskHistoryRepository.CreateAsync(taskHistory);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Message.Should().Be(null);
        _mockTaskHistoryCollection.Verify(x => x.InsertOneAsync(It.IsAny<TaskHistory>(), null, default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Failed_To_Insert_By_Exception()
    {
        // Arrange
        var taskHistory = new TaskHistory("Tarefa atualizada", Guid.NewGuid(), Guid.NewGuid());

        _mockTaskHistoryCollection
            .Setup(x => x.InsertOneAsync(It.IsAny<TaskHistory>(), null, default))
            .ThrowsAsync(new Exception("Erro ao inserir TaskHistory"));

        // Act
        var result = await _taskHistoryRepository.CreateAsync(taskHistory);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Falha ao gravar histórico da tarefa.");
        _mockTaskHistoryCollection.Verify(x => x.InsertOneAsync(It.IsAny<TaskHistory>(), null, default), Times.Once);
    }
}
