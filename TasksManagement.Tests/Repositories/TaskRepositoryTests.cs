using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Enums;
using TasksManagement.Infrastructure.Database;
using TasksManagement.Infrastructure.Repositories;

namespace TasksManagement.Tests.Repositories;

public class TaskRepositoryTests
{
    private readonly DbContextOptions<SqlDbContext> _dbContextOptions;

    public TaskRepositoryTests()
    {
        // Configura o banco de dados em memória
        _dbContextOptions = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Task_When_Id_Exists()
    {
        // Arrange
        var task = new TaskItem("Tarefa Teste", "Descrição", DateTime.Now, ETaskPriority.High, Guid.NewGuid());

        // Adiciona a tarefa ao banco em memória
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
        }

        // Instancia o repositório com o contexto real
        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetByIdAsync(task.Id);

        // Assert
        result.IsValid.Should().BeTrue("A tarefa deve ser retornada com sucesso.");
        result.Data.Should().NotBeNull("A tarefa deve ser retornada se o ID existir");
        result.Data!.Title.Should().Be("Tarefa Teste", "O título da tarefa deve corresponder ao esperado");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Null_With_Success_When_Id_Does_Not_Exist()
    {
        // Arrange
        var nonExistentTaskId = Guid.NewGuid();
        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetByIdAsync(nonExistentTaskId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Message.Should().Be(null);
    }

    [Fact]
    public async Task CreateAsync_Should_Save_Task_Successfully()
    {
        // Arrange
        var task = new TaskItem("Nova Tarefa", "Descrição", DateTime.Now.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());
        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.CreateAsync(task);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após salvar a tarefa.");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var task = new TaskItem("Nova Tarefa", "Descrição", DateTime.Now.AddDays(1), ETaskPriority.Medium, Guid.NewGuid());

        var mockDbContext = new Mock<SqlDbContext>(_dbContextOptions);
        mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Erro ao salvar a tarefa"));

        var repository = new TaskRepository(mockDbContext.Object);

        // Act
        var result = await repository.CreateAsync(task);

        // Assert
        result.IsValid.Should().BeFalse("Deve retornar falha quando ocorrer uma exceção.");
        result.Message.Should().Contain("Erro ao criar tarefa", "Erro ao criar tarefa.");
    }

    [Fact]
    public async Task UpdateAsync_Should_Update_Task_Successfully()
    {

        // Arrange
        var task = new TaskItem("Tarefa Teste", "Descrição", DateTime.Now, ETaskPriority.High, Guid.NewGuid());

        // Adiciona a tarefa ao banco em memória
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
        }

        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Atualiza a tarefa
        task.Update("Tarefa Atualizada", "Descrição Atualizada", DateTime.Now.AddDays(2), ETaskStatus.InProgress);

        // Act
        var result = await repository.UpdateAsync(task);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após atualizar a tarefa.");
    }

    [Fact]
    public async Task DeleteAsync_Should_Remove_Task_Successfully()
    {
        // Arrange
        var task = new TaskItem("Tarefa Teste", "Descrição", DateTime.Now, ETaskPriority.High, Guid.NewGuid());

        // Adiciona a tarefa ao banco em memória
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Tasks.Add(task);
            await context.SaveChangesAsync();
        }

        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.DeleteAsync(task);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após deletar a tarefa.");
    }

    [Fact]
    public async Task GetCompletedTasksByProjectIdAsync_Should_Return_Completed_Tasks()
    {
        // Arrange
        var project = new Project("Projeto Teste", Guid.NewGuid());

        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa Concluída", "Descrição", DateTime.Now.AddDays(5), ETaskPriority.Medium, project.Id)
        };

        tasks.ForEach(t => t.Update(t.Title, t.Description, t.DueDate, ETaskStatus.Completed));

        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Tasks.AddRange(tasks);
            await context.SaveChangesAsync();
        }

        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetCompletedTasksByProjectIdAsync(new List<Project> { project }, DateTime.Now.AddDays(-30));

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar tarefas concluídas.");
        result.Data.Should().HaveCount(1, "Deve retornar 1 tarefa concluída");
        result.Data.First().Title.Should().Be("Tarefa Concluída", "O título da tarefa deve ser 'Tarefa Concluída'");
    }

    [Fact]
    public async Task GetByProjectIdAsync_Should_Return_Tasks_For_Project()
    {
        // Arrange
        var projectId = Guid.NewGuid();
        var tasks = new List<TaskItem>
        {
            new TaskItem("Tarefa 1", "Descrição 1", DateTime.Now.AddDays(1), ETaskPriority.Medium, projectId),
            new TaskItem("Tarefa 2", "Descrição 2", DateTime.Now.AddDays(2), ETaskPriority.High, projectId)
        };

        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Tasks.AddRange(tasks);
            await context.SaveChangesAsync();
        }

        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetByProjectIdAsync(projectId);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar tarefas para o projeto.");
        result.Data.Should().HaveCount(2, "Deve retornar 2 tarefas para o projeto");
        result.Data.First().Title.Should().Be("Tarefa 1", "O título da primeira tarefa deve ser 'Tarefa 1'");
    }

    [Fact]
    public async Task GetCompletedTasksByProjectIdAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var project = new Project("Projeto Teste", Guid.NewGuid());
        var projects = new List<Project> { project };
        var dateThreshold = DateTime.Now;

        // Cria um mock de DbSet que lança uma exceção ao acessar o método Provider
        var mockSet = new Mock<DbSet<TaskItem>>();
        mockSet.As<IQueryable<TaskItem>>().Setup(m => m.Provider).Throws(new Exception("Erro ao buscar tarefas concluídas"));

        var mockDbContext = new Mock<SqlDbContext>(_dbContextOptions);
        mockDbContext.Setup(c => c.Set<TaskItem>()).Returns(mockSet.Object);

        var repository = new TaskRepository(mockDbContext.Object);

        // Act
        var result = await repository.GetCompletedTasksByProjectIdAsync(projects, dateThreshold);

        // Assert
        result.IsValid.Should().BeFalse("Deve retornar falha quando ocorrer uma exceção.");
        result.Message.Should().Contain("Erro ao buscar tarefas concluídas", "Erro ao buscar tarefas concluídas.");
    }




    [Fact]
    public async Task GetTopTasksWithMostCommentsAsync_Should_Return_Top_Tasks()
    {
        // Arrange
        var task = new TaskItem("Tarefa Teste", "Descrição", DateTime.Now, ETaskPriority.High, Guid.NewGuid());
        var comments = new List<Comment>
        {
            new Comment("Comentário 1", task.Id, Guid.NewGuid()),
            new Comment("Comentário 2", task.Id, Guid.NewGuid())
        };

        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Tasks.Add(task);
            context.Comments.AddRange(comments);
            await context.SaveChangesAsync();
        }

        var repository = new TaskRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetTopTasksWithMostCommentsAsync(DateTime.Now.AddDays(-30));

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar tarefas com mais comentários.");
        result.Data.Should().HaveCount(1, "Deve retornar 1 tarefa com mais comentários");
        result.Data.First().Title.Should().Be("Tarefa Teste", "O título da tarefa deve ser 'Tarefa Teste'");
    }

    [Fact]
    public async Task GetTopTasksWithMostCommentsAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var dateThreshold = DateTime.Now;

        var mockSet = new Mock<DbSet<Comment>>();
        mockSet.As<IQueryable<Comment>>().Setup(m => m.Provider).Throws(new Exception("Erro ao buscar as tarefas com mais comentários"));

        var mockDbContext = new Mock<SqlDbContext>(_dbContextOptions);
        mockDbContext.Setup(c => c.Set<Comment>()).Returns(mockSet.Object);

        var repository = new TaskRepository(mockDbContext.Object);

        // Act
        var result = await repository.GetTopTasksWithMostCommentsAsync(dateThreshold);

        // Assert
        result.IsValid.Should().BeFalse();
        result.Message.Should().Contain("Erro ao buscar as tarefas com mais comentários");
    }



}



