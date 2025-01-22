using FluentAssertions;
using Moq;
using TasksManagement.API.Models.InputModels.Project;
using TasksManagement.API.Models.InputModels.User;
using TasksManagement.Application.Services;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;
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
    public async Task GetByIdAsync_Should_Return_Success_When_Project_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Projeto Teste", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(project));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetByIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().NotBeNull();
        result.Data!.Name.Should().Be("Projeto Teste");
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
    public async Task GetAllByUserIdAsync_Should_Return_Success_When_Projects_Found()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projects = new List<Project>
        {
            new Project("Projeto 1", userId),
            new Project("Projeto 2", userId)
        };

        _mockProjectRepository
            .Setup(repo => repo.GetAllByUserIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success((IEnumerable<Project>)projects));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetAllByUserIdAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data!.First().Name.Should().Be("Projeto 1");
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

    [Fact]
    public async Task GetTasksByProjectIdAsync_Should_Return_Failure_When_Project_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<Project>("Projeto não encontrado.", statusCode: 404));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetTasksByProjectIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Projeto não encontrado.");
        _mockProjectRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetTasksByProjectIdAsync_Should_Return_Failure_When_Tasks_Not_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Projeto Teste", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(project));

        _mockTaskRepository
            .Setup(repo => repo.GetByProjectIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<IEnumerable<TaskItem>>("Falha ao obter tarefa.", statusCode: 500));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetTasksByProjectIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Falha ao obter tarefa.");
        _mockTaskRepository.Verify(repo => repo.GetByProjectIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task GetTasksByProjectIdAsync_Should_Return_Success_When_Tasks_Found()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Projeto Teste", Guid.NewGuid());
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa 1", "Descrição 1", DateTime.Now.AddDays(1), ETaskPriority.Medium, projectId),
            new TaskItem("Tarefa 2", "Descrição 2", DateTime.Now.AddDays(2), ETaskPriority.High, projectId)
        };

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(project));

        _mockTaskRepository
            .Setup(repo => repo.GetByProjectIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success((IEnumerable<TaskItem>)tasks));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetTasksByProjectIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Data.Should().HaveCount(2);
        result.Data!.First().Title.Should().Be("Tarefa 1");
        _mockTaskRepository.Verify(repo => repo.GetByProjectIdAsync(It.IsAny<Guid>()), Times.Once);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Project_Validation_Fails()
    {
        // Arrange
        var inputModel = new CreateProjectInputModel(
            new UserInputModel(Guid.NewGuid(), "Teste", "Gerente"),
            "" // Nome do projeto inválido
        );

        var user = new User("Teste", "Gerente");

        _mockUserService
            .Setup(service => service.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(user));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Erro ao validar o projeto");
        _mockProjectRepository.Verify(repo => repo.CreateAsync(It.IsAny<Project>()), Times.Never);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Project_Creation_Fails()
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
            .ReturnsAsync(Result.Failure("Erro ao criar o projeto"));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.CreateAsync(inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Erro ao criar o projeto");
        _mockProjectRepository.Verify(repo => repo.CreateAsync(It.IsAny<Project>()), Times.Once);
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Project_Validation_Fails()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var inputModel = new UpdateProjectInputModel(""); // Nome do projeto inválido

        var existingProject = new Project("Existing Project", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(existingProject));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.UpdateAsync(projectId, inputModel);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Ocorreu um erro inesperado");
    }


    [Fact]
    public async Task DeleteAsync_Should_Return_Failure_When_Project_Validation_For_Delete_Fails()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var existingProject = new Project("Existing Project", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Success(existingProject));

        existingProject.AddNotification("Erro", "Ocorreu um erro inesperado");

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.DeleteAsync(projectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Ocorreu um erro inesperado");
    }

    [Fact]
    public async Task GetTasksByProjectIdAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ThrowsAsync(new Exception("Erro ao buscar projeto"));

        var service = new ProjectService(_mockUserService.Object, _mockProjectRepository.Object, _mockTaskRepository.Object);

        // Act
        var result = await service.GetTasksByProjectIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Ocorreu um erro inesperado");
        _mockProjectRepository.Verify(repo => repo.GetByIdAsync(It.IsAny<Guid>()), Times.Once);
    }


}