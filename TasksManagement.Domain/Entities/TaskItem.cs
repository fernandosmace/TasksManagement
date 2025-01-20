using Flunt.Validations;
using TasksManagement.Domain.Enums;

namespace TasksManagement.Domain.Entities;
public class TaskItem : Entity
{
    public string Title { get; private set; }
    public string Description { get; private set; }
    public DateTime DueDate { get; private set; }
    public ETaskStatus Status { get; private set; }
    public ETaskPriority Priority { get; private set; }
    public Guid ProjectId { get; private set; }

    private readonly List<Comment> _comments = [];
    public IReadOnlyCollection<Comment> Comments => _comments.AsReadOnly();

    private readonly List<TaskHistory> _history = [];
    public IReadOnlyCollection<TaskHistory> History => _history.AsReadOnly();

    public TaskItem(string title, string description, DateTime dueDate, ETaskPriority priority, Guid projectId)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        ProjectId = projectId;
        Status = ETaskStatus.Pending;

        Validar();
    }

    public void Update(string title, string description, DateTime dueDate, ETaskStatus status)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;

        Validar();
    }

    protected override void Validar()
    {
        AddNotifications(new Contract<TaskItem>()
            .Requires()
            .IsNotNullOrWhiteSpace(Title, nameof(Title), $"Campo {nameof(Title)} não foi informado.")
            .IsNotNullOrWhiteSpace(Description, nameof(Description), $"Campo {nameof(Description)} não foi informado.")
            .IsGreaterOrEqualsThan(DueDate, DateTime.UtcNow, nameof(DueDate), $"Campo {nameof(DueDate)} deve ser uma data futura."));
    }
}