namespace TasksManagement.Domain.Entities;
public class TaskHistory
{
    public Guid Id { get; set; }
    public Guid TaskId { get; set; }
    public string Changes { get; set; }
    public DateTime ChangedAt { get; set; }
    public Guid ChangedBy { get; set; }

    public TaskHistory(string changes, Guid taskId, Guid changedBy)
    {
        Id = Guid.NewGuid();
        Changes = changes;
        TaskId = taskId;
        ChangedBy = changedBy;
        ChangedAt = DateTime.UtcNow;
    }
}