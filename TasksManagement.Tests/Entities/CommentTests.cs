using FluentAssertions;
using TasksManagement.Domain.Entities;
using TasksManagement.Tests.Mocks.Entities;

namespace TasksManagement.Tests.Entities;
public class CommentTests
{
    [Fact]
    public void Comment_Should_Create_Valid_Comment()
    {
        // Arrange & Act
        var comment = CommentMock.GetDefault();

        // Assert
        comment.IsValid.Should().BeTrue("O comentário deve ser válido");
        comment.TaskId.Should().NotBeEmpty();
        comment.UserId.Should().NotBeEmpty();
        comment.Notifications.Count().Should().Be(0);
        comment.CreatedAt.Should().BeCloseTo(DateTime.UtcNow, precision: TimeSpan.FromSeconds(1));
    }

    [Fact]
    public void Comment_Should_Fail_When_Content_Is_Empty()
    {
        // Arrange & Act
        var comment = CommentMock.GetWithEmptyContent();

        // Assert
        comment.IsValid.Should().BeFalse("O comentário não deve ser válido sem conteúdo");
        comment.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(Comment.Content) &&
            n.Message == $"Campo {nameof(Comment.Content)} não foi informado.");
    }

    [Fact]
    public void Comment_Should_Fail_When_Content_Is_Null()
    {
        // Arrange & Act
        var comment = CommentMock.GetWithNullContent();

        // Assert
        comment.IsValid.Should().BeFalse("O comentário não deve ser válido sem conteúdo");
        comment.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(Comment.Content) &&
            n.Message == $"Campo {nameof(Comment.Content)} não foi informado.");
    }

    [Fact]
    public void Comment_Should_Fail_When_Content_Is_WhiteSpace()
    {
        // Arrange & Act
        var comment = CommentMock.GetWithWhiteSpaceContent();

        // Assert
        comment.IsValid.Should().BeFalse("O comentário não deve ser válido sem conteúdo");
        comment.Notifications.Should().ContainSingle(n =>
            n.Key == nameof(Comment.Content) &&
            n.Message == $"Campo {nameof(Comment.Content)} não foi informado.");
    }
}