using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TasksManagement.Domain.Entities;
using TasksManagement.Infrastructure.Database;
using TasksManagement.Infrastructure.Repositories;

namespace TasksManagement.Tests.Repositories;

public class UserRepositoryTests
{
    private readonly DbContextOptions<SqlDbContext> _dbContextOptions;

    public UserRepositoryTests()
    {
        // Configura o banco de dados em memória
        _dbContextOptions = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_User_When_Id_Exists()
    {
        // Arrange
        var user = new User("Usuario Teste", "usuario@teste.com");

        // Adiciona o usuário ao banco em memória
        using (var context = new SqlDbContext(_dbContextOptions))
        {
            context.Users.Add(user);
            await context.SaveChangesAsync();
        }

        // Instancia o repositório com o contexto real
        var repository = new UserRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetByIdAsync(user.Id);

        // Assert
        result.IsValid.Should().BeTrue("O usuário deve ser retornado com sucesso.");
        result.Data.Should().NotBeNull("O usuário deve ser retornado se o ID existir");
        result.Data!.Name.Should().Be("Usuario Teste", "O nome do usuário deve corresponder ao esperado");
    }

    [Fact]
    public async Task GetByIdAsync_Should_Return_Failure_When_Id_Does_Not_Exist()
    {
        // Arrange
        var nonExistentUserId = Guid.NewGuid();
        var repository = new UserRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.GetByIdAsync(nonExistentUserId);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Message.Should().Be(null);
    }

    [Fact]
    public async Task CreateAsync_Should_Save_User_Successfully()
    {
        // Arrange
        var user = new User("Novo Usuário", "novo@teste.com");
        var repository = new UserRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.CreateAsync(user);

        // Assert
        result.IsValid.Should().BeTrue("Deve retornar sucesso após salvar o usuário.");
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var user = new User("Novo Usuário", "novo@teste.com");

        var mockDbContext = new Mock<SqlDbContext>(_dbContextOptions);
        mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Erro ao salvar o usuário"));

        var repository = new UserRepository(mockDbContext.Object);

        // Act
        var result = await repository.CreateAsync(user);

        // Assert
        result.IsValid.Should().BeFalse("Deve retornar falha quando ocorrer uma exceção.");
        result.Message.Should().Contain("Erro ao criar usuário", "Erro ao criar usuário.");
    }
}

