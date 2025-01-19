using Flunt.Validations;
using System.Text;
using TasksManagement.Domain.Enums;
using TasksManagement.Domain.Events;

namespace TasksManagement.Domain.Entities;
public class Task : Entity
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

    public Task(string title, string description, DateTime dueDate, ETaskPriority priority, Guid projectId)
    {
        Title = title;
        Description = description;
        DueDate = dueDate;
        Priority = priority;
        ProjectId = projectId;
        Status = ETaskStatus.Pending;

        AddNotifications(new Contract<Task>()
            .Requires()
            .IsNotNullOrWhiteSpace(Title, nameof(Title), $"Campo {nameof(Title)} não foi informado.")
            .IsNotNullOrWhiteSpace(Description, nameof(Description), $"Campo {nameof(Description)} não foi informado.")
            .IsGreaterOrEqualsThan(DueDate, DateTime.UtcNow, nameof(DueDate), $"Campo {nameof(DueDate)} deve ser uma data futura."));
    }
    public void AddComment(Comment comment) => _comments.Add(comment);

    public void Update(string title, string description, DateTime dueDate, ETaskStatus status, Guid userId)
    {
        var changes = new StringBuilder();

        if (Title != title)
            changes.AppendLine($"Título atualizado de '{Title}' para '{title}'. ");
        if (Description != description)
            changes.AppendLine($"Descrição atualizada de '{Description}' para '{description}'. ");
        if (DueDate != dueDate)
            changes.AppendLine($"Data de entrega atualizada de '{DueDate}' para '{dueDate}'. ");
        if (Status != status)
            changes.AppendLine($"Status alterado de '{Status}' para '{status}'. ");

        Title = title;
        Description = description;
        DueDate = dueDate;
        Status = status;

        DomainEvents.Raise(new TaskUpdatedEvent(Id, changes.ToString().Trim(), DateTime.UtcNow, userId));
    }
}