using Flunt.Validations;

namespace TasksManagement.Domain.Entities;
public class TaskHistory : Entity
{
    public Guid TaskId { get; private set; }
    public string Changes { get; private set; }
    public DateTime ChangedAt { get; private set; }
    public Guid ChangedBy { get; private set; }

    public TaskHistory(string changes, Guid taskId, Guid changedBy)
    {
        Changes = changes;
        TaskId = taskId;
        ChangedBy = changedBy;
        ChangedAt = DateTime.UtcNow;

        AddNotifications(new Contract<TaskHistory>()
            .Requires()
            .IsNotNullOrEmpty(Changes, nameof(Changes), $"Campo {nameof(Changes)} não foi informado."));
    }
}