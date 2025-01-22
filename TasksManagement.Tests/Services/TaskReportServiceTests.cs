using FluentAssertions;
using Moq;
using TasksManagement.Application.Services;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Models.ReportModels;

namespace TasksManagement.Tests.Services;

public class TaskReportServiceTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<ITaskRepository> _mockTaskRepository;
    private readonly TaskReportService _service;

    public TaskReportServiceTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockTaskRepository = new Mock<ITaskRepository>();
        _service = new TaskReportService(_mockProjectRepository.Object, _mockTaskRepository.Object);
    }

    [Fact]
    public async Task GenerateCompletedTasksByUserReportAsync_Should_Return_Empty_When_No_Projects()
    {
        // Arrange
        var userId = Guid.NewGuid();
        _mockProjectRepository
            .Setup(repo => repo.GetAllByUserIdAsync(userId))
            .ReturnsAsync(Result.Failure<IEnumerable<Project>>("Nenhum projeto encontrado."));

        // Act
        var result = await _service.GenerateCompletedTasksByUserReportAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().BeEmpty();
        result.Message.Should().Be(null);
        _mockProjectRepository.Verify(repo => repo.GetAllByUserIdAsync(userId), Times.Once);
    }

    [Fact]
    public async Task GenerateCompletedTasksByUserReportAsync_Should_Return_Reports_When_Projects_And_Tasks_Exist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var project = new Project("Project 1", userId);
        var task1 = new TaskItem("Task 1", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Medium, project.Id);
        var task2 = new TaskItem("Task 2", "description", DateTime.UtcNow.AddDays(1), ETaskPriority.Low, project.Id);

        task1.Update(task1.Title, task1.Description, task1.DueDate, ETaskStatus.Completed);
        task2.Update(task2.Title, task2.Description, task2.DueDate, ETaskStatus.Completed);

        var completedTasks = new List<TaskItem> { task1, task2 };

        _mockProjectRepository
            .Setup(repo => repo.GetAllByUserIdAsync(userId))
            .ReturnsAsync(Result.Success<IEnumerable<Project>>(new[] { project }));

        _mockTaskRepository
            .Setup(repo => repo.GetCompletedTasksByProjectIdAsync(It.IsAny<IEnumerable<Project>>(), It.IsAny<DateTime>()))
            .ReturnsAsync(Result.Success<IEnumerable<TaskItem>>(completedTasks));

        // Act
        var result = await _service.GenerateCompletedTasksByUserReportAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().ContainSingle(r => r.Title == "Task 1");
        result.Data.Should().ContainSingle(r => r.Title == "Task 2");
        result.Message.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateCompletedTasksByUserReportAsync_Should_Return_Failure_When_Getting_Tasks_Fails()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var project = new Project("Project 1", userId);

        _mockProjectRepository
            .Setup(repo => repo.GetAllByUserIdAsync(userId))
            .ReturnsAsync(Result.Success<IEnumerable<Project>>(new[] { project }));
        _mockTaskRepository
            .Setup(repo => repo.GetCompletedTasksByProjectIdAsync(It.IsAny<IEnumerable<Project>>(), It.IsAny<DateTime>()))
            .ReturnsAsync(Result.Failure<IEnumerable<TaskItem>>("Erro ao gerar relatório."));

        // Act
        var result = await _service.GenerateCompletedTasksByUserReportAsync(userId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Erro ao gerar relatório.");
    }

    [Fact]
    public async Task GenerateTopTasksByCommentsAsync_Should_Return_Reports_When_Tasks_Exist()
    {
        // Arrange
        var tasksWithComments = new List<TaskWithCommentCountReportModel>
        {
            new TaskWithCommentCountReportModel { TaskId = Guid.NewGuid(), Title = "Task 1", CommentCount = 5 },
            new TaskWithCommentCountReportModel { TaskId = Guid.NewGuid(), Title = "Task 2", CommentCount = 3 }
        };

        _mockTaskRepository
            .Setup(repo => repo.GetTopTasksWithMostCommentsAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(Result.Success<IEnumerable<TaskWithCommentCountReportModel>>(tasksWithComments));

        // Act
        var result = await _service.GenerateTopTasksByCommentsAsync();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().ContainSingle(r => r.Title == "Task 1" && r.CommentCount == 5);
        result.Data.Should().ContainSingle(r => r.Title == "Task 2" && r.CommentCount == 3);
        result.Message.Should().BeNullOrEmpty();
    }

    [Fact]
    public async Task GenerateTopTasksByCommentsAsync_Should_Return_Failure_When_Getting_Tasks_Fails()
    {
        // Arrange
        _mockTaskRepository
            .Setup(repo => repo.GetTopTasksWithMostCommentsAsync(It.IsAny<DateTime>()))
            .ReturnsAsync(Result.Failure<IEnumerable<TaskWithCommentCountReportModel>>("Erro ao gerar relatório."));

        // Act
        var result = await _service.GenerateTopTasksByCommentsAsync();

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Erro ao gerar relatório.");
    }
}
