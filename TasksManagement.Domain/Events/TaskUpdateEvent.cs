using MediatR;

namespace TasksManagement.Domain.Events;
public class TaskUpdatedEvent : INotification
{
    public Guid TaskId { get; }
    public string Changes { get; }
    public DateTime ChangedAt { get; }
    public Guid ChangedBy { get; }

    public TaskUpdatedEvent(Guid taskId, string changes, DateTime changedAt, Guid changedBy)
    {
        TaskId = taskId;
        Changes = changes;
        ChangedAt = changedAt;
        ChangedBy = changedBy;
    }
}