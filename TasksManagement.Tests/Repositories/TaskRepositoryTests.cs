using FluentAssertions;
using Moq;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;
using TasksManagement.Domain.Interfaces.Repositories;

namespace TasksManagement.Tests.Repositories;
public class TaskRepositoryTests
{
    private readonly Mock<ITaskRepository> _mockTaskRepository;

    public TaskRepositoryTests()
    {
        _mockTaskRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Task_When_Id_Exists()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem("Tarefa Teste", "Descrição", DateTime.Now, ETaskPriority.High, Guid.NewGuid());

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == taskId)))
            .ReturnsAsync(Result.Success(task));

        var repository = _mockTaskRepository.Object;

        // Act
        var result = await repository.GetByIdAsync(taskId);

        // Assert
        result.IsValid.Should().BeTrue("A tarefa deve ser retornada com sucesso.");
        result.Data.Should().NotBeNull("A tarefa deve ser retornada se o ID existir");
        result.Data.Title.Should().Be("Tarefa Teste", "O título da tarefa deve corresponder ao esperado");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Id_Does_Not_Exist()
    {
        // Arrange
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<TaskItem>("Tarefa não encontrada"));

        var repository = _mockTaskRepository.Object;

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsValid.Should().BeFalse("Deve falhar caso a tarefa não exista");
        result.Message.Should().Be("Tarefa não encontrada", "A mensagem de erro deve ser a esperada.");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == taskId)))
            .ThrowsAsync(new Exception("Erro ao acessar o banco de dados"));

        var repository = _mockTaskRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.GetByIdAsync(taskId));

        // Verifica a mensagem da exceção
        exception.Message.Should().Be("Erro ao acessar o banco de dados", "A mensagem da exceção deve ser a esperada.");
    }

    [Fact]
    public async Task CreateAsync_Should_Save_Task_Successfully()
    {
        // Arrange
        var task = new TaskItem("Nova Tarefa", "Descrição", DateTime.Now.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());

        _mockTaskRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(Result.Success("Tarefa criada com sucesso."));

        var repository = _mockTaskRepository.Object;

        // Act
        var result = await repository.CreateAsync(task);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após salvar a tarefa.");
        _mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskItem>()), Times.Once, "O método CreateAsync deve ser chamado uma vez");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var task = new TaskItem("Nova Tarefa", "Descrição", DateTime.Now.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());

        _mockTaskRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<TaskItem>()))
            .ThrowsAsync(new Exception("Erro ao salvar a tarefa"));

        var repository = _mockTaskRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.CreateAsync(task));

        exception.Message.Should().Be("Erro ao salvar a tarefa", "A mensagem da exceção deve ser a esperada.");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Task_Successfully()
    {
        // Arrange
        var task = new TaskItem("Tarefa Atualizada", "Descrição", DateTime.Now.AddDays(2), ETaskPriority.Low, Guid.NewGuid());

        _mockTaskRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(Result.Success("Tarefa atualizada com sucesso."));

        var repository = _mockTaskRepository.Object;

        // Act
        var result = await repository.UpdateAsync(task);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após atualizar a tarefa.");
        _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Once, "O método UpdateAsync deve ser chamado uma vez");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Task_Successfully()
    {
        // Arrange
        var task = new TaskItem("Tarefa Deletada", "Descrição", DateTime.Now.AddDays(3), ETaskPriority.High, Guid.NewGuid());

        _mockTaskRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(Result.Success("Tarefa deletada com sucesso."));

        var repository = _mockTaskRepository.Object;

        // Act
        var result = await repository.DeleteAsync(task);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após deletar a tarefa.");
        _mockTaskRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TaskItem>()), Times.Once, "O método DeleteAsync deve ser chamado uma vez");
    }

    [Fact]
    public async Task GetCompletedTasksByProjectIdAsync_Should_Return_Completed_Tasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa Concluída", "Descrição", DateTime.Now.AddDays(5), ETaskPriority.Medium, projectId)
        };

        tasks.ForEach(t => t.Update(t.Title, t.Description, t.DueDate, ETaskStatus.Completed));

        _mockTaskRepository
            .Setup(repo => repo.GetCompletedTasksByProjectIdAsync(It.IsAny<IEnumerable<Project>>(), It.IsAny<DateTime>()))
            .ReturnsAsync(Result.Success((IEnumerable<TaskItem>)tasks));

        var repository = _mockTaskRepository.Object;

        // Act
        var result = await repository.GetCompletedTasksByProjectIdAsync(new List<Project>(), DateTime.Now.AddDays(-1));

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar tarefas concluídas.");
        result.Data.Should().HaveCount(1, "Deve retornar 1 tarefa concluída");
        result.Data.First().Title.Should().Be("Tarefa Concluída", "O título da tarefa deve ser 'Tarefa Concluída'");
    }
}
