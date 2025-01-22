using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;
using TasksManagement.Infrastructure.Database;
using TasksManagement.Infrastructure.Repositories;

namespace TasksManagement.Tests.Repositories;

public class ProjectRepositoryTests
{
    private readonly DbContextOptions<SqlDbContext> _dbContextOptions;

    public ProjectRepositoryTests()
    {
        // Configura o banco de dados em memória
        _dbContextOptions = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Project_When_Id_Exists()
    {
        // Arrange
        var project = new Project("Projeto Teste", Guid.NewGuid());

        // Adiciona o projeto ao banco em memória
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        // Instancia o repositório com o contexto real
        var repository = new ProjectRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetByIdAsync(project.Id);

        // Assert
        result.IsValid.Should().BeTrue("O projeto deve ser retornado com sucesso.");
        result.Data.Should().NotBeNull("O projeto deve ser retornado se o ID existir");
        result.Data!.Name.Should().Be("Projeto Teste", "O nome do projeto deve corresponder ao esperado");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Id_Does_Not_Exist()
    {
        // Arrange
        var nonExistentProjectId = Guid.NewGuid();
        var repository = new ProjectRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetByIdAsync(nonExistentProjectId);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Be("Projeto não encontrado.");
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

        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Projects.AddRange(projects);
            await context.SaveChangesAsync();
        }

        var repository = new ProjectRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetAllByUserIdAsync(userId);

        // Assert
        result.IsValid.Should().BeTrue("O retorno deve ser um sucesso");
        result.Data.Should().HaveCount(2, "Devem ser retornados dois projetos para o usuário");
        result.Data.First().Name.Should().Be("Projeto 1", "O nome do primeiro projeto deve ser 'Projeto 1'");
    }

    [Fact]
    public async Task CreateAsync_Should_Save_Project_Successfully()
    {
        // Arrange
        var project = new Project("Novo Projeto", Guid.NewGuid());
        var repository = new ProjectRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.CreateAsync(project);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após salvar o projeto.");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var project = new Project("Novo Projeto", Guid.NewGuid());

        var mockDbContext = new Mock<SqlDbContext>(_dbContextOptions);
        mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Erro ao salvar o projeto"));

        var repository = new ProjectRepository(mockDbContext.Object);

        // Act
        var result = await repository.CreateAsync(project);

        // Assert
        result.IsValid.Should().BeFalse("Deve retornar falha quando ocorrer uma exceção.");
        result.Message.Should().Contain("Erro ao criar o projeto", "Erro ao criar o projeto.");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Project_Successfully()
    {
        // Arrange
        var project = new Project("Projeto Teste", Guid.NewGuid());

        // Adiciona o projeto ao banco em memória
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        var repository = new ProjectRepository(new SqlDbContext(_dbContextOptions));

        // Atualiza o projeto
        project.Update("Projeto Atualizado");

        // Act
        var result = await repository.UpdateAsync(project);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após atualizar o projeto.");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Project_Successfully()
    {
        // Arrange
        var project = new Project("Projeto Teste", Guid.NewGuid());

        // Adiciona o projeto ao banco em memória
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Projects.Add(project);
            await context.SaveChangesAsync();
        }

        var repository = new ProjectRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.DeleteAsync(project);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após deletar o projeto.");
    }

    [Fact]
    public async Task GetTopProjectsWithMostCompletedTasksAsync_Should_Return_Top_Projects()
    {
        // Arrange
        var project = new Project("Projeto Teste", Guid.NewGuid());

        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa Concluída", "Descrição", DateTime.Now.AddDays(5), ETaskPriority.Medium, project.Id)
        };

        tasks.ForEach(t => t.Update(t.Title, t.Description, t.DueDate, ETaskStatus.Completed));
        tasks.ForEach(t => t.GetType().GetProperty("CompletionDate")?.SetValue(t, DateTime.Now));

        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Projects.Add(project);
            context.Tasks.AddRange(tasks);
            await context.SaveChangesAsync();
        }

        var repository = new ProjectRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetTopProjectsWithMostCompletedTasksAsync(30);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar projetos com mais tarefas concluídas.");
        result.Data.Should().HaveCount(1, "Deve retornar 1 projeto com mais tarefas concluídas");
        result.Data.First().Name.Should().Be("Projeto Teste", "O nome do projeto deve ser 'Projeto Teste'");
    }
}

