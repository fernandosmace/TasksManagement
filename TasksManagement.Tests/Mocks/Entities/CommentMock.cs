using TasksManagement.Domain.Entities;

namespace TasksManagement.Tests.Mocks.Entities;
public static class CommentMock
{
    public static Comment GetDefault() => new Comment("Comentário válido", Guid.NewGuid(), Guid.NewGuid());
    public static Comment GetWithEmptyContent() => new Comment("", Guid.NewGuid(), Guid.NewGuid());
    public static Comment GetWithNullContent() => new Comment(null, Guid.NewGuid(), Guid.NewGuid());
    public static Comment GetWithWhiteSpaceContent() => new Comment(" ", Guid.NewGuid(), Guid.NewGuid());
}
