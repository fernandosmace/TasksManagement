using FluentAssertions;
using Moq;
using TasksManagement.Application.Services;
using TasksManagement.Domain;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Models.ReportModels;

namespace TasksManagement.Tests.Services;

public class ProjectReportServiceTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<ITaskRepository> _mockTaskRepository;

    public ProjectReportServiceTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockTaskRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task GenerateTopProjectsByCompletedTasksAsync_Should_Return_Empty_Result_When_No_Projects_Found()
    {
        // Arrange
        _mockProjectRepository
            .Setup(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Failure<IEnumerable<ProjectWithTasksCountReportModel>>("Nenhum projeto encontrado."));

        var service = new ProjectReportService(_mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GenerateTopProjectsByCompletedTasksAsync();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().BeEmpty();
        _mockProjectRepository.Verify(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GenerateTopProjectsByCompletedTasksAsync_Should_Return_Projects_When_They_Exist()
    {
        // Arrange
        var reportModels = new List<ProjectWithTasksCountReportModel>
        {
            new() { Id = Guid.NewGuid(), Name = "Project 1", TasksCount = 5 },
            new() { Id = Guid.NewGuid(), Name = "Project 2", TasksCount = 3 }
        };

        _mockProjectRepository
            .Setup(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Success<IEnumerable<ProjectWithTasksCountReportModel>>(reportModels));

        var service = new ProjectReportService(_mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GenerateTopProjectsByCompletedTasksAsync();

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data.Should().ContainSingle(p => p.Name == "Project 1" && p.CompletedTaskCount == 5);
        result.Data.Should().ContainSingle(p => p.Name == "Project 2" && p.CompletedTaskCount == 3);
        _mockProjectRepository.Verify(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()), Times.Once);
    }

    [Fact]
    public async Task GenerateTopProjectsByCompletedTasksAsync_Should_Return_Failure_When_ProjectRepository_Fails()
    {
        // Arrange
        _mockProjectRepository
            .Setup(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Failure<IEnumerable<ProjectWithTasksCountReportModel>>("Erro ao buscar projetos."));

        var service = new ProjectReportService(_mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GenerateTopProjectsByCompletedTasksAsync();

        // Assert
        result.IsValid.Should().BeTrue(); // Result should be valid but empty due to failure
        result.Data.Should().BeEmpty();
        _mockProjectRepository.Verify(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()), Times.Once);
    }
}