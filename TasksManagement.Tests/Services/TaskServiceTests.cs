using FluentAssertions;
using Moq;
using System.Reflection;
using TasksManagement.API.Models.InputModels.Task;
using TasksManagement.API.Models.InputModels.User;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;
using TasksManagement.Services;

namespace TasksManagement.Tests.Services;

public class TaskServiceTests
{
    private readonly Mock<IProjectService> _mockProjectService;
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly TaskService _service;

    public TaskServiceTests()
    {
        _mockProjectService = new Mock<IProjectService>();
        _mockUserService = new Mock<IUserService>();
        _mockTaskRepository = new Mock<ITaskRepository>();
        _service = new TaskService(_mockProjectService.Object, _mockUserService.Object, _mockTaskRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Task_Not_Found()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Failure<TaskItem>("Tarefa não encontrada.", statusCode: 404));

        // Act
        var result = await _service.GetByIdAsync(taskId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Tarefa não encontrada.");
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Success_When_Task_Found()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Success(task));

        // Act
        var result = await _service.GetByIdAsync(taskId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().Be(task);
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Project_Not_Found()
    {
        // Arrange
        var inputModel = new CreateTaskInputModel("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());
        _mockProjectService
            .Setup(service => service.GetByIdAsync(inputModel.ProjectId))
            .ReturnsAsync(Result.Failure<Project>("Projeto não encontrado.", statusCode: 404));

        // Act
        var result = await _service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Projeto não encontrado.");
        _mockProjectService.Verify(service => service.GetByIdAsync(inputModel.ProjectId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Too_Many_Pending_Tasks()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Project 1", projectId);
        var tasks = Enumerable.Range(1, 20).Select(i => new TaskItem($"Task {i}", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, projectId)).ToList();

        // Usando reflection para definir a propriedade privada _tasks
        var tasksField = typeof(Project).GetField("_tasks", BindingFlags.NonPublic | BindingFlags.Instance);
        tasksField.SetValue(project, tasks);

        var inputModel = new CreateTaskInputModel("Task 21", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, projectId);
        _mockProjectService
            .Setup(service => service.GetByIdAsync(inputModel.ProjectId))
            .ReturnsAsync(Result.Success(project));

        // Act
        var result = await _service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Não é possível adicionar mais de 20 tarefas por projeto. Finalize ou remova tarefas existentes para adicionar uma nova tarefa.");
        _mockProjectService.Verify(service => service.GetByIdAsync(inputModel.ProjectId), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Success_When_Task_Created()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Project 1", projectId);
        var inputModel = new CreateTaskInputModel("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, projectId);
        var task = new TaskItem(inputModel.Title!, inputModel.Description!, inputModel.DueDate, inputModel.Priority, inputModel.ProjectId);

        _mockProjectService
            .Setup(service => service.GetByIdAsync(inputModel.ProjectId))
            .ReturnsAsync(Result.Success(project));
        _mockTaskRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockProjectService.Verify(service => service.GetByIdAsync(inputModel.ProjectId), Times.Once);
        _mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Task_Not_Found()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var inputModel = new UpdateTaskInputModel(new UserInputModel(Guid.NewGuid(), "User", "Role"), "Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskStatus.Completed);
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Failure<TaskItem>("Tarefa não encontrada.", statusCode: 404));

        // Act
        var result = await _service.UpdateAsync(taskId, inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Tarefa não encontrado.");
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_User_Not_Found()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());
        var inputModel = new UpdateTaskInputModel(new UserInputModel(Guid.NewGuid(), "User", "Role"), "Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskStatus.Completed);

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Success(task));
        _mockUserService
            .Setup(service => service.GetByIdAsync(inputModel.User.Id))
            .ReturnsAsync(Result.Failure<User>("Usuário não encontrado.", statusCode: 404));

        // Act
        var result = await _service.UpdateAsync(taskId, inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Usuário não encontrado.");
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
        _mockUserService.Verify(service => service.GetByIdAsync(inputModel.User.Id), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Success_When_Task_Updated()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());
        var inputModel = new UpdateTaskInputModel(new UserInputModel(Guid.NewGuid(), "User", "Role"), "Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskStatus.Completed);
        var user = new User("User", "Role");

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Success(task));
        _mockUserService
            .Setup(service => service.GetByIdAsync(inputModel.User.Id))
            .ReturnsAsync(Result.Success(user));
        _mockTaskRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.UpdateAsync(taskId, inputModel);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
        _mockUserService.Verify(service => service.GetByIdAsync(inputModel.User.Id), Times.Once);
        _mockTaskRepository.Verify(repo => repo.UpdateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_Failure_When_Task_Not_Found()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Failure<TaskItem>("Tarefa não encontrada.", statusCode: 404));

        // Act
        var result = await _service.DeleteAsync(taskId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Tarefa não encontrada.");
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_Success_When_Task_Deleted()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Success(task));
        _mockTaskRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(Result.Success());

        // Act
        var result = await _service.DeleteAsync(taskId);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
        _mockTaskRepository.Verify(repo => repo.DeleteAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Task_Validation_Fails()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Project 1", projectId);
        var inputModel = new CreateTaskInputModel("", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, projectId); // Título inválido

        _mockProjectService
            .Setup(service => service.GetByIdAsync(inputModel.ProjectId))
            .ReturnsAsync(Result.Success(project));

        var service = new TaskService(_mockProjectService.Object, _mockUserService.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Erro ao validar a Task");
        _mockProjectService.Verify(service => service.GetByIdAsync(inputModel.ProjectId), Times.Once);
        _mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskItem>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Task_Creation_Fails()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Project 1", projectId);
        var inputModel = new CreateTaskInputModel("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, projectId);
        var task = new TaskItem(inputModel.Title!, inputModel.Description!, inputModel.DueDate, inputModel.Priority, inputModel.ProjectId);

        _mockProjectService
            .Setup(service => service.GetByIdAsync(inputModel.ProjectId))
            .ReturnsAsync(Result.Success(project));
        _mockTaskRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<TaskItem>()))
            .ReturnsAsync(Result.Failure("Erro ao criar a tarefa."));

        var service = new TaskService(_mockProjectService.Object, _mockUserService.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Erro ao criar a tarefa.");
        _mockProjectService.Verify(service => service.GetByIdAsync(inputModel.ProjectId), Times.Once);
        _mockTaskRepository.Verify(repo => repo.CreateAsync(It.IsAny<TaskItem>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Task_Validation_Fails()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var task = new TaskItem("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());
        var inputModel = new UpdateTaskInputModel(new UserInputModel(Guid.NewGuid(), "User", "Role"), "", "description", DateTime.UtcNow.AddDays(1), ETaskStatus.Completed); // Título inválido

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ReturnsAsync(Result.Success(task));
        _mockUserService
            .Setup(service => service.GetByIdAsync(inputModel.User.Id))
            .ReturnsAsync(Result.Success(new User("User", "Role")));

        var service = new TaskService(_mockProjectService.Object, _mockUserService.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.UpdateAsync(taskId, inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Ocorreu um erro inesperado");
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var taskId = Guid.NewGuid();
        var inputModel = new UpdateTaskInputModel(new UserInputModel(Guid.NewGuid(), "User", "Role"), "Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskStatus.Completed);

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ThrowsAsync(new Exception("Erro ao buscar tarefa"));

        var service = new TaskService(_mockProjectService.Object, _mockUserService.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.UpdateAsync(taskId, inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Ocorreu um erro inesperado");
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var taskId = Guid.NewGuid();

        _mockTaskRepository
            .Setup(repo => repo.GetByIdAsync(taskId))
            .ThrowsAsync(new Exception("Erro ao buscar tarefa"));

        var service = new TaskService(_mockProjectService.Object, _mockUserService.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.DeleteAsync(taskId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Ocorreu um erro inesperado");
        _mockTaskRepository.Verify(repo => repo.GetByIdAsync(taskId), Times.Once);
    }
}