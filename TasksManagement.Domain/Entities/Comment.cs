using Flunt.Validations;

namespace TasksManagement.Domain.Entities;
public class Comment : Entity
{
    public Guid TaskId { get; private set; }
    public string Content { get; private set; }
    public DateTime CreatedAt { get; private set; }
    public Guid UserId { get; private set; }

    public Comment(string content, Guid taskId, Guid userId)
    {
        Content = content;
        TaskId = taskId;
        UserId = userId;
        CreatedAt = DateTime.UtcNow;

        Validar();
    }

    protected override void Validar()
    {
        AddNotifications(new Contract<Comment>()
            .Requires()
            .IsNotNullOrWhiteSpace(Content, nameof(Content), $"Campo {nameof(Content)} não foi informado."));
    }
}