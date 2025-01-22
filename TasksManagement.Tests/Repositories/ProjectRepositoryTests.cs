using FluentAssertions;
using Moq;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;
using TasksManagement.Domain.Models.ReportModels;

namespace TasksManagement.Tests.Repositories;
public class ProjectRepositoryTests
{
    private readonly Mock<IProjectRepository> _mockProjectRepository;

    public ProjectRepositoryTests()
    {
        _mockProjectRepository = new Mock<IProjectRepository>();
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Project_When_Id_Exists()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var project = new Project("Projeto Teste", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == projectId)))
            .ReturnsAsync(Result.Success(project));

        var repository = _mockProjectRepository.Object;

        // Act
        var result = await repository.GetByIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeTrue("O projeto deve ser retornado com sucesso.");
        result.Data.Should().NotBeNull("O projeto deve ser retornado se o ID existir");
        result.Data.Name.Should().Be("Projeto Teste", "O nome do projeto deve corresponder ao esperado");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Id_Does_Not_Exist()
    {
        // Arrange
        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(Result.Failure<Project>("Projeto não encontrado"));

        var repository = _mockProjectRepository.Object;

        // Act
        var result = await repository.GetByIdAsync(Guid.NewGuid());

        // Assert
        result.IsValid.Should().BeFalse("Deve falhar caso o projeto não exista");
        result.Message.Should().Be("Projeto não encontrado", "A mensagem de erro deve ser a esperada.");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var projectId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(repo => repo.GetByIdAsync(It.Is<Guid>(id => id == projectId)))
            .ThrowsAsync(new Exception("Erro ao acessar o banco de dados"));

        var repository = _mockProjectRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.GetByIdAsync(projectId));

        // Verifica a mensagem da exceção
        exception.Message.Should().Be("Erro ao acessar o banco de dados", "A mensagem da exceção deve ser a esperada.");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Return_Projects_For_User()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var projects = new List<Project>
            {
                new Project("Projeto 1", userId),
                new Project("Projeto 2", userId)
            };

        _mockProjectRepository
            .Setup(repo => repo.GetAllByUserIdAsync(It.Is<Guid>(id => id == userId)))
            .ReturnsAsync(Result.Success((IEnumerable<Project>)projects));

        var repository = _mockProjectRepository.Object;

        // Act
        var result = await repository.GetAllByUserIdAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue("O retorno deve ser um sucesso");
        result.Data.Should().HaveCount(2, "Devem ser retornados dois projetos para o usuário");
        result.Data.First().Name.Should().Be("Projeto 1", "O nome do primeiro projeto deve ser 'Projeto 1'");
    }

    [Fact]
    public async Task GetAllByUserIdAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var userId = Guid.NewGuid();

        _mockProjectRepository
            .Setup(repo => repo.GetAllByUserIdAsync(It.Is<Guid>(id => id == userId)))
            .ThrowsAsync(new Exception("Erro ao acessar os projetos do usuário"));

        var repository = _mockProjectRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.GetAllByUserIdAsync(userId));

        // Verifica a mensagem da exceção
        exception.Message.Should().Be("Erro ao acessar os projetos do usuário", "A mensagem da exceção deve ser a esperada.");
    }

    [Fact]
    public async Task CreateAsync_Should_Save_Project_Successfully()
    {
        // Arrange
        var project = new Project("Novo Projeto", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Success("Projeto criado com sucesso."));

        var repository = _mockProjectRepository.Object;

        // Act
        var result = await repository.CreateAsync(project);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após salvar o projeto.");
        _mockProjectRepository.Verify(repo => repo.CreateAsync(It.IsAny<Project>()), Times.Once, "O método CreateAsync deve ser chamado uma vez");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var project = new Project("Novo Projeto", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Project>()))
            .ThrowsAsync(new Exception("Erro ao salvar o projeto"));

        var repository = _mockProjectRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.CreateAsync(project));

        exception.Message.Should().Be("Erro ao salvar o projeto", "A mensagem da exceção deve ser a esperada.");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Project_Successfully()
    {
        // Arrange
        var project = new Project("Projeto Atualizado", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Success("Projeto atualizado com sucesso."));

        var repository = _mockProjectRepository.Object;

        // Act
        var result = await repository.UpdateAsync(project);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após atualizar o projeto.");
        _mockProjectRepository.Verify(repo => repo.UpdateAsync(It.IsAny<Project>()), Times.Once, "O método UpdateAsync deve ser chamado uma vez");
    }

    [Fact]
    public async Task UpdateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var project = new Project("Projeto Atualizado", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.UpdateAsync(It.IsAny<Project>()))
            .ThrowsAsync(new Exception("Erro ao atualizar o projeto"));

        var repository = _mockProjectRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.UpdateAsync(project));

        // Verifica a mensagem da exceção
        exception.Message.Should().Be("Erro ao atualizar o projeto", "A mensagem da exceção deve ser a esperada.");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Project_Successfully()
    {
        // Arrange
        var project = new Project("Projeto Removido", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<Project>()))
            .ReturnsAsync(Result.Success("Projeto deletado com sucesso."));

        var repository = _mockProjectRepository.Object;

        // Act
        var result = await repository.DeleteAsync(project);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após deletar o projeto.");
        _mockProjectRepository.Verify(repo => repo.DeleteAsync(It.IsAny<Project>()), Times.Once, "O método DeleteAsync deve ser chamado uma vez");
    }

    [Fact]
    public async Task DeleteAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var project = new Project("Projeto Removido", Guid.NewGuid());

        _mockProjectRepository
            .Setup(repo => repo.DeleteAsync(It.IsAny<Project>()))
            .ThrowsAsync(new Exception("Erro ao deletar o projeto"));

        var repository = _mockProjectRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.DeleteAsync(project));

        // Verifica a mensagem da exceção
        exception.Message.Should().Be("Erro ao deletar o projeto", "A mensagem da exceção deve ser a esperada.");
    }

    [Fact]
    public async Task GetTopProjectsWithMostCompletedTasksAsync_Should_Return_Top_Projects()
    {
        // Arrange
        var projectsWithTasks = new List<ProjectWithTasksCountReportModel>
            {
                new ProjectWithTasksCountReportModel { Id = Guid.NewGuid(), Name = "Projeto 1", TasksCount = 10 },
                new ProjectWithTasksCountReportModel { Id = Guid.NewGuid(), Name = "Projeto 2", TasksCount = 5 }
            };

        _mockProjectRepository
            .Setup(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()))
            .ReturnsAsync(Result.Success((IEnumerable<ProjectWithTasksCountReportModel>)projectsWithTasks));

        var repository = _mockProjectRepository.Object;

        // Act
        var result = await repository.GetTopProjectsWithMostCompletedTasksAsync(30);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso ao buscar projetos com mais tarefas concluídas.");
        result.Data.Should().HaveCount(2, "Devem ser retornados dois projetos com mais tarefas concluídas");
        result.Data.First().Name.Should().Be("Projeto 1", "O nome do primeiro projeto deve ser 'Projeto 1'");
    }

    [Fact]
    public async Task GetTopProjectsWithMostCompletedTasksAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        _mockProjectRepository
            .Setup(repo => repo.GetTopProjectsWithMostCompletedTasksAsync(It.IsAny<int>()))
            .ThrowsAsync(new Exception("Erro ao buscar projetos com tarefas completadas"));

        var repository = _mockProjectRepository.Object;

        // Act & Assert
        var exception = await Assert.ThrowsAsync<Exception>(async () => await repository.GetTopProjectsWithMostCompletedTasksAsync(30));

        // Verifica a mensagem da exceção
        exception.Message.Should().Be("Erro ao buscar projetos com tarefas completadas", "A mensagem da exceção deve ser a esperada.");
    }
}