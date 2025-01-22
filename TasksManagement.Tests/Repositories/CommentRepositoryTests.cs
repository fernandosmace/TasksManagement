using FluentAssertions;
using Moq;
using TasksManagement.Domain;
using TasksManagement.Domain.Entities;
using TasksManagement.Domain.Interfaces.Repositories;

namespace TasksManagement.Tests.Repositories;
public class CommentRepositoryTests
{
    private readonly Mock<ICommentRepository> _mockCommentRepository;

    public CommentRepositoryTests()
    {
        _mockCommentRepository = new Mock<ICommentRepository>();
    }

    [Fact]
    public async Task CreateAsync_Should_Save_Comment_Successfully()
    {
        // Arrange
        var comment = new Comment("Este é um comentário", Guid.NewGuid(), Guid.NewGuid());

        _mockCommentRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Comment>()))
            .ReturnsAsync(Result.Success("Comentário criado com sucesso"));

        var repository = _mockCommentRepository.Object;

        // Act
        var result = await repository.CreateAsync(comment);

        // Assert
        result.IsValid.Should().BeTrue("O resultado da criação deve ser bem-sucedido.");
        result.Message.Should().Be("Comentário criado com sucesso", "A mensagem de sucesso deve ser a esperada.");
        result.Data.Should().BeNull("Não há dados adicionais retornados.");
        _mockCommentRepository.Verify(repo => repo.CreateAsync(It.IsAny<Comment>()), Times.Once, "O método CreateAsync deve ser chamado uma vez.");
    }

    [Fact]
    public async Task CreateAsync_Should_Fail_When_Repository_Throws_Exception()
    {
        // Arrange
        var comment = new Comment("Este é um comentário", Guid.NewGuid(), Guid.NewGuid());

        _mockCommentRepository
            .Setup(repo => repo.CreateAsync(It.IsAny<Comment>()))
            .ReturnsAsync(Result.Failure("Erro ao salvar o comentário"));

        var repository = _mockCommentRepository.Object;

        // Act
        var result = await repository.CreateAsync(comment);

        // Assert
        result.IsValid.Should().BeFalse("O resultado da criação deve falhar.");
        result.Message.Should().Be("Erro ao salvar o comentário", "A mensagem de erro deve ser a esperada.");
        result.Data.Should().BeNull("Não há dados adicionais retornados.");
        _mockCommentRepository.Verify(repo => repo.CreateAsync(It.IsAny<Comment>()), Times.Once, "O método CreateAsync deve ser chamado uma vez.");
    }
}