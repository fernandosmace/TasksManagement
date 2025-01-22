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
        // Mockar a coleção de TaskHistories
        _mockTaskHistoryCollection = new Mock<IMongoCollection<TaskHistory>>();

        // Mockar o MongoDbContext (interface), retornando a coleção mockada
        _mockMongoDbContext = new Mock<IMongoDbContext>();
        _mockMongoDbContext.Setup(m => m.TaskHistories).Returns(_mockTaskHistoryCollection.Object);

        // Instanciar o repositório com o contexto mockado
        _taskHistoryRepository = new TaskHistoryRepository(_mockMongoDbContext.Object);
    }

    [Fact]
    public async Task CreateAsync_Should_Insert_TaskHistory_Successfully()
    {
        // Arrange
        var taskHistory = new TaskHistory("Task status updated", Guid.NewGuid(), Guid.NewGuid());

        // Simular o comportamento de sucesso da inserção
        _mockTaskHistoryCollection
            .Setup(x => x.InsertOneAsync(It.IsAny<TaskHistory>(), null, default))
            .Returns(Task.CompletedTask);

        // Act
        Func<Task> act = async () => await _taskHistoryRepository.CreateAsync(taskHistory);

        // Assert
        await act.Should().NotThrowAsync("Deve salvar o TaskHistory com sucesso");
        _mockTaskHistoryCollection.Verify(x => x.InsertOneAsync(It.IsAny<TaskHistory>(), null, default), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Throw_Exception_When_Failed_To_Insert()
    {
        // Arrange
        var taskHistory = new TaskHistory("Task status updated", Guid.NewGuid(), Guid.NewGuid());

        // Simular uma falha ao tentar inserir
        _mockTaskHistoryCollection
            .Setup(x => x.InsertOneAsync(It.IsAny<TaskHistory>(), null, default))
            .ThrowsAsync(new Exception("Erro ao inserir TaskHistory"));

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await _taskHistoryRepository.CreateAsync(taskHistory));
        exception.Message.Should().Be("Erro ao inserir TaskHistory");
    }
}