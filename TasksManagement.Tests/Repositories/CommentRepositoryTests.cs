using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using Moq;
using TasksManagement.Domain.Entities;
using TasksManagement.Infrastructure.Database;
using TasksManagement.Infrastructure.Repositories;

namespace TasksManagement.Tests.Repositories;

public class CommentRepositoryTests
{
    private readonly DbContextOptions<SqlDbContext> _dbContextOptions;

    public CommentRepositoryTests()
    {
        // Configura o banco de dados em memória
        _dbContextOptions = new DbContextOptionsBuilder<SqlDbContext>()
            .UseInMemoryDatabase("TestDatabase")
            .Options;
    }

    [Fact]
    public async Task CreateAsync_Should_Save_Comment_Successfully()
    {
        // Arrange
        var comment = new Comment("Este é um comentário", Guid.NewGuid(), Guid.NewGuid());
        var repository = new CommentRepository(new SqlDbContext(_dbContextOptions));

        // Act
        var result = await repository.CreateAsync(comment);

        // Assert
        result.IsValid.Should().BeTrue();
        result.Message.Should().Be(null);
    }

    [Fact]
    public async Task CreateAsync_Should_Return_Failure_When_Exception_Occurs()
    {
        // Arrange
        var comment = new Comment("Este é um comentário", Guid.NewGuid(), Guid.NewGuid());

        var mockDbContext = new Mock<SqlDbContext>(_dbContextOptions);
        mockDbContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>())).ThrowsAsync(new Exception("Erro ao salvar o comentário"));

        var repository = new CommentRepository(mockDbContext.Object);

        // Act
        var result = await repository.CreateAsync(comment);

        // Assert
        result.IsValid.Should().BeFalse("O resultado da criação deve falhar.");
        result.Message.Should().Be(null);
    }
}


