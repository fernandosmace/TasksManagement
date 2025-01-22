using FluentAssertions;
using Moq;
using TasksManagement.API.Models.InputModels.Project;
using TasksManagement.API.Models.InputModels.User;
using TasksManagement.Application.Services;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Interfaces.Services;

namespace TasksManagement.Tests.Services;

public class ProjectServiceTests
{
    private readonly Mock<IUserService> _mockUserService;
    private readonly Mock<IProjectRepository> _mockProjectRepository;
    private readonly Mock<ITaskRepository> _mockTaskRepository;

    public ProjectServiceTests()
    {
        _mockUserService = new Mock<IUserService>();
        _mockProjectRepository = new Mock<IProjectRepository>();
        _mockTaskRepository = new Mock<ITaskRepository>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Project_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<Project>("Projeto não encontrado.", statusCode: 404));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetByIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Projeto não encontrado.");
        _mockProjectRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Return_Failure_When_No_Project_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(repo => repo.GetAllByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<IEnumerable<Project>>("Nenhum projeto encontrado para o usuário.", statusCode: 404));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetAllByUserIdAsync(userId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Nenhum projeto encontrado para o usuário.");
        _mockProjectRepository.Verify(repo => repo.GetAllByUserIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_User_Creation_Fails()
    {
        // Arrange
        var inputModel = new CreateProjectInputModel(
            new UserInputModel(Guid.NewGuid(), "Teste", "Gerente"),
            "Novo Projeto"
        );

        _mockUserService
            .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<User>("Usuário não encontrado."));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Ocorreu um erro inesperado");
        _mockUserService.Verify(service => service.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Success_When_Project_Created_Successfully()
    {
        // Arrange
        var inputModel = new CreateProjectInputModel(
            new UserInputModel(Guid.NewGuid(), "Teste", "Gerente"),
            "Novo Projeto"
        );

        var user = new User("Teste", "Gerente");
        var project = new Project(inputModel.Name!, user.Id);

        _mockUserService
            .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(user));

        _mockProjectRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Success());

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockProjectRepository.Verify(repo => repo.CreateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Project_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var inputModel = new UpdateProjectInputModel("Updated Project");

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<Project>("Projeto não encontrado.", statusCode: 404));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.UpdateAsync(projectId, inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Projeto não encontrado.");
        _mockProjectRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Update_Fails()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var inputModel = new UpdateProjectInputModel("Updated Project");

        var existingProject = new Project("Existing Project", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(existingProject));

        _mockProjectRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Failure("Erro ao atualizar o projeto", statusCode: 500));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.UpdateAsync(projectId, inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Erro ao atualizar o projeto");
        _mockProjectRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Success_When_Project_Updated_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var inputModel = new UpdateProjectInputModel("Updated Project");

        var existingProject = new Project("Existing Project", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(existingProject));

        _mockProjectRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Success("Projeto atualizado com sucesso."));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.UpdateAsync(projectId, inputModel);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockProjectRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_Failure_When_Project_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<Project>("Projeto não encontrado.", statusCode: 404));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.DeleteAsync(projectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Projeto não encontrado.");
        _mockProjectRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_Failure_When_Delete_Fails()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project("Existing Project", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(existingProject));

        _mockProjectRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Failure("Erro ao durante a exclusão do projeto", statusCode: 500));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.DeleteAsync(projectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Erro ao durante a exclusão do projeto");
        _mockProjectRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_Success_When_Project_Deleted_Successfully()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project("Existing Project", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(existingProject));

        _mockProjectRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Success("Projeto deletado com sucesso."));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.DeleteAsync(projectId);

        // Assert
        result.IsValid.Should().BeTrue();
        _mockProjectRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Project>()), Times.Once);
    }
}